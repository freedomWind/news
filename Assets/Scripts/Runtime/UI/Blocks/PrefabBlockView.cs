using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Rendering;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class PrefabBlockView : BlockViewBase
    {
        private PrefabRenderDescriptor descriptor;
        private Transform prefabHost;

        public static BlockViewBase Create(Transform parent, PrefabRenderDescriptor prefabDescriptor)
        {
            var root = AppUIFactory.CreateImage("PrefabBlock", parent, AppTheme.SurfaceMuted);
            var view = root.gameObject.AddComponent<PrefabBlockView>();
            view.descriptor = prefabDescriptor;
            root.gameObject.name = string.IsNullOrWhiteSpace(prefabDescriptor?.type)
                ? "PrefabBlock"
                : "PrefabBlock_" + prefabDescriptor.type;

            AppUIFactory.AddLayoutElement(root.gameObject, 96f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 8f, new RectOffset(14, 14, 14, 14));
            view.prefabHost = root.transform;
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            ClearHost();

            var prefabKey = ResolvePrefabKey(data);
            if (string.IsNullOrWhiteSpace(prefabKey))
            {
                RenderFallback("Missing prefab key.");
                return;
            }

            var prefab = Resources.Load<GameObject>(prefabKey);
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab block missing prefabKey={prefabKey} type={data?.type}");
                RenderFallback("Missing prefab: " + prefabKey);
                return;
            }

            var instance = Instantiate(prefab, prefabHost, false);
            var boundView = ResolveBoundView(instance);
            if (boundView == null)
            {
                Debug.LogWarning($"Prefab block has no IDataBoundView<BlockData>: {prefabKey}");
                DestroyInstance(instance);
                RenderFallback("Prefab binding missing: " + prefabKey);
                return;
            }

            boundView.Bind(data, OnAction);
        }

        private string ResolvePrefabKey(BlockData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.prefabKey))
            {
                return data.prefabKey;
            }

            return descriptor != null ? descriptor.prefabKey : string.Empty;
        }

        private void RenderFallback(string message)
        {
            var fallbackType = descriptor != null ? descriptor.fallbackType : string.Empty;
            var fallbackMessage = string.IsNullOrWhiteSpace(fallbackType)
                ? message
                : message + " fallback=" + fallbackType;

            var fallbackLabel = AppUIFactory.CreateText(
                "PrefabFallback",
                prefabHost,
                fallbackMessage,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(fallbackLabel.gameObject, 64f);
        }

        private void ClearHost()
        {
            if (prefabHost == null)
            {
                return;
            }

            for (var i = prefabHost.childCount - 1; i >= 0; i--)
            {
                var child = prefabHost.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private static IDataBoundView<BlockData> ResolveBoundView(GameObject instance)
        {
            if (instance == null)
            {
                return null;
            }

            var components = instance.GetComponents<MonoBehaviour>();
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is IDataBoundView<BlockData> boundView)
                {
                    return boundView;
                }
            }

            return null;
        }

        private static void DestroyInstance(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(instance);
            }
            else
            {
                DestroyImmediate(instance);
            }
        }
    }
}
