using System;
using System.IO;
using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Blocks;
using NewsFramework.UI.Rendering;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.Editor.PrefabScaffolder
{
    public sealed class BlockPrefabScaffolderWindow : EditorWindow
    {
        private const string DefaultExportFolder = "Assets/RuntimeResources/Resources/Prefabs/Blocks/Generated";
        private int selectedTypeIndex;
        private int selectedSampleSourceIndex;
        private string prefabName = "GeneratedBlockPrefab";
        private string exportFolder = DefaultExportFolder;
        private BlockData sampleData;
        private GameObject previewCanvas;
        private GameObject previewRoot;
        private string validationMessage = "No preview generated.";
        private Vector2 scroll;

        [MenuItem("NewsFramework/Prefab Scaffolder/BlockData To Prefab")]
        public static void Open() => GetWindow<BlockPrefabScaffolderWindow>("Block Prefab Scaffolder");

        private void OnEnable()
        {
            RefreshSample();
        }

        private void OnDisable()
        {
            DestroyPreview();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawControls();
            EditorGUILayout.Space(8f);
            DrawValidation();
            EditorGUILayout.EndScrollView();
        }

        private void DrawControls()
        {
            EditorGUILayout.LabelField("Block Prefab Scaffolder", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Generates a new prefab scaffold from BlockData sample data. It does not overwrite designer-edited prefabs unless you explicitly choose an existing file name.",
                MessageType.Info);

            EditorGUI.BeginChangeCheck();
            selectedTypeIndex = EditorGUILayout.Popup("Block Type", selectedTypeIndex, BlockSampleCatalog.BlockTypes);
            selectedSampleSourceIndex = EditorGUILayout.Popup(
                "Sample Source",
                selectedSampleSourceIndex,
                BlockSampleCatalog.SampleSources);
            if (EditorGUI.EndChangeCheck())
            {
                RefreshSample();
            }

            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
            exportFolder = EditorGUILayout.TextField("Export Folder", exportFolder);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Sample"))
                {
                    RefreshSample();
                }

                if (GUILayout.Button("Preview"))
                {
                    BuildPreview();
                }

                if (GUILayout.Button("Export Prefab"))
                {
                    ExportPrefab();
                }
            }
        }

        private void DrawValidation()
        {
            EditorGUILayout.LabelField("Sample Data", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("id", sampleData != null ? sampleData.id : string.Empty);
                EditorGUILayout.TextField("type", sampleData != null ? sampleData.type : string.Empty);
                EditorGUILayout.TextField("title", sampleData != null ? sampleData.title : string.Empty);
                EditorGUILayout.TextField("subtitle", sampleData != null ? sampleData.subtitle : string.Empty);
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(validationMessage, MessageType.None);
        }

        private void RefreshSample()
        {
            var type = BlockSampleCatalog.BlockTypes[Mathf.Clamp(selectedTypeIndex, 0, BlockSampleCatalog.BlockTypes.Length - 1)];
            sampleData = BlockSampleCatalog.CreateSample(type, selectedSampleSourceIndex);
            prefabName = ToPascalCase(type) + "Scaffold";
            validationMessage = "Sample ready. Click Preview to generate a temporary UI tree.";
        }

        private void BuildPreview()
        {
            DestroyPreview();
            previewCanvas = AppUIFactory.CreateOverlayCanvas("BlockPrefabScaffoldPreview").gameObject;
            previewRoot = CreateScaffoldRoot(sampleData);
            previewRoot.transform.SetParent(previewCanvas.transform, false);
            validationMessage = ValidateScaffold(previewRoot, sampleData, out _);
            Selection.activeGameObject = previewRoot;
        }

        private void ExportPrefab()
        {
            if (sampleData == null)
            {
                validationMessage = "Cannot export without sample data.";
                return;
            }

            var root = CreateScaffoldRoot(sampleData);
            var validation = ValidateScaffold(root, sampleData, out var boundView);
            if (boundView == null)
            {
                DestroyImmediate(root);
                validationMessage = validation;
                return;
            }

            if (!TryResolveResourceKey(exportFolder, SanitizeFileName(prefabName), out var resourceKey, out var resourceError))
            {
                DestroyImmediate(root);
                validationMessage = resourceError;
                return;
            }

            Directory.CreateDirectory(exportFolder);
            AssetDatabase.Refresh();

            var prefabPath = Path.Combine(exportFolder, SanitizeFileName(prefabName) + ".prefab").Replace("\\", "/");
            if (File.Exists(prefabPath) &&
                !EditorUtility.DisplayDialog(
                    "Overwrite prefab?",
                    "Prefab already exists:\n" + prefabPath + "\n\nFirst version is intended for new scaffolds. Continue only if you explicitly want to replace it.",
                    "Overwrite",
                    "Cancel"))
            {
                DestroyImmediate(root);
                validationMessage = "Export cancelled. Existing prefab was not overwritten.";
                return;
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            DestroyImmediate(root);
            AssetDatabase.Refresh();

            validationMessage = "Exported prefab: " + prefabPath + "\nResource key: " + resourceKey;
        }

        private static GameObject CreateScaffoldRoot(BlockData data)
        {
            var root = new GameObject("BlockScaffold_" + data.type, typeof(RectTransform));
            var view = root.AddComponent<GenericBlockPrefabView>();
            view.Bind(data, action =>
            {
                var actionType = action != null ? action.type : "none";
                var actionTarget = action != null ? action.target : string.Empty;
                Debug.Log("Scaffold action: " + actionType + " -> " + actionTarget);
            });
            return root;
        }

        private static string ValidateScaffold(
            GameObject root,
            BlockData data,
            out IDataBoundView<BlockData> boundView)
        {
            boundView = ResolveBoundView(root);
            if (root == null)
            {
                return "Invalid scaffold: root is missing.";
            }

            if (data == null || string.IsNullOrWhiteSpace(data.type))
            {
                return "Invalid scaffold: sample BlockData is missing type.";
            }

            if (boundView == null)
            {
                return "Invalid scaffold: root has no IDataBoundView<BlockData> binding component.";
            }

            if (root.GetComponent<Button>() == null)
            {
                return "Invalid scaffold: root has no Button action binding point.";
            }

            if (!HasBindingLabel(root.transform, "Title") || !HasBindingLabel(root.transform, "Subtitle"))
            {
                return "Invalid scaffold: required text binding labels are missing.";
            }

            return "OK: scaffold root has IDataBoundView<BlockData>, text binding labels, and a Button action binding point for sample type '" + data.type + "'.";
        }

        private static IDataBoundView<BlockData> ResolveBoundView(GameObject root)
        {
            if (root == null)
            {
                return null;
            }

            var components = root.GetComponents<MonoBehaviour>();
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is IDataBoundView<BlockData> boundView)
                {
                    return boundView;
                }
            }

            return null;
        }

        private static bool HasBindingLabel(Transform root, string labelName)
        {
            var child = root != null ? root.Find(labelName) : null;
            return child != null && child.GetComponent("TextMeshProUGUI") != null;
        }

        private void DestroyPreview()
        {
            if (previewRoot != null)
            {
                DestroyImmediate(previewRoot);
            }

            if (previewCanvas != null)
            {
                DestroyImmediate(previewCanvas);
            }

            previewRoot = null;
            previewCanvas = null;
        }

        private static string ToPascalCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Block";
            }

            var parts = value.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = string.Empty;
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                result += char.ToUpperInvariant(part[0]) + (part.Length > 1 ? part.Substring(1) : string.Empty);
            }

            return result;
        }

        private static string SanitizeFileName(string value)
        {
            var name = string.IsNullOrWhiteSpace(value) ? "GeneratedBlockPrefab" : value.Trim();
            foreach (var invalid in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalid, '_');
            }

            return name;
        }

        private static bool TryResolveResourceKey(
            string folder,
            string prefabFileName,
            out string resourceKey,
            out string error)
        {
            resourceKey = string.Empty;
            error = string.Empty;

            var normalizedFolder = (folder ?? string.Empty).Replace("\\", "/").TrimEnd('/');
            if (string.IsNullOrWhiteSpace(normalizedFolder) || !normalizedFolder.StartsWith("Assets/", StringComparison.Ordinal))
            {
                error = "Export folder must be a project-relative Assets path.";
                return false;
            }

            const string resourcesSegment = "/Resources/";
            var resourcesIndex = normalizedFolder.IndexOf(resourcesSegment, StringComparison.Ordinal);
            if (resourcesIndex < 0)
            {
                error = "Export folder must be inside a Resources folder so runtime can load it by prefabKey.";
                return false;
            }

            var relativeFolder = normalizedFolder.Substring(resourcesIndex + resourcesSegment.Length);
            resourceKey = string.IsNullOrEmpty(relativeFolder)
                ? prefabFileName
                : relativeFolder + "/" + prefabFileName;
            return true;
        }
    }
}
