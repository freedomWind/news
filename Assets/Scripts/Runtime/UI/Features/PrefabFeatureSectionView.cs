using System;
using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Rendering;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class PrefabFeatureSectionView : FeatureSectionViewBase
    {
        private PrefabRenderDescriptor descriptor;
        private Func<Transform, FeatureSectionViewBase> fallbackFactory;

        public static FeatureSectionViewBase Create(
            Transform parent,
            PrefabRenderDescriptor prefabDescriptor,
            Func<Transform, FeatureSectionViewBase> fallback)
        {
            var root = AppUIFactory.CreateRect("PrefabFeatureSection", parent);
            var view = root.gameObject.AddComponent<PrefabFeatureSectionView>();
            view.descriptor = prefabDescriptor;
            view.fallbackFactory = fallback;
            root.gameObject.name = string.IsNullOrWhiteSpace(prefabDescriptor?.type)
                ? "PrefabFeatureSection"
                : "PrefabFeatureSection_" + prefabDescriptor.type;
            AppUIFactory.AddLayoutElement(root.gameObject, 56f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 0f, new RectOffset(0, 0, 0, 0));
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            ClearHost();

            var prefabKey = ResolvePrefabKey(data);
            if (string.IsNullOrWhiteSpace(prefabKey))
            {
                RenderFallback(data, "Missing feature prefab key.");
                return;
            }

            var prefab = Resources.Load<GameObject>(prefabKey);
            if (prefab == null)
            {
                Debug.LogWarning($"Feature section prefab missing prefabKey={prefabKey} type={data?.type}");
                RenderFallback(data, "Missing feature prefab: " + prefabKey);
                return;
            }

            var instance = Instantiate(prefab, transform, false);
            var boundView = ResolveBoundView(instance);
            if (boundView == null)
            {
                Debug.LogWarning($"Feature section prefab has no IDataBoundView<FeatureSectionData>: {prefabKey}");
                DestroyInstance(instance);
                RenderFallback(data, "Feature prefab binding missing: " + prefabKey);
                return;
            }

            boundView.Bind(data, OnAction);
            SyncLayoutFrom(instance);
        }

        private string ResolvePrefabKey(FeatureSectionData data)
        {
            if (data != null && !string.IsNullOrWhiteSpace(data.prefabKey))
            {
                return data.prefabKey;
            }

            return descriptor != null ? descriptor.prefabKey : string.Empty;
        }

        private void RenderFallback(FeatureSectionData data, string message)
        {
            if (fallbackFactory != null)
            {
                var fallback = fallbackFactory(transform);
                fallback.Bind(data, OnAction);
                SyncLayoutFrom(fallback.gameObject);
                return;
            }

            var fallbackLabel = AppUIFactory.CreateText(
                "FeaturePrefabFallback",
                transform,
                message,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(fallbackLabel.gameObject, 56f);
            SyncLayoutFrom(fallbackLabel.gameObject);
        }

        private void SyncLayoutFrom(GameObject source)
        {
            var sourceLayout = source != null ? source.GetComponent<LayoutElement>() : null;
            if (sourceLayout == null)
            {
                return;
            }

            var targetLayout = GetComponent<LayoutElement>();
            if (targetLayout == null)
            {
                targetLayout = gameObject.AddComponent<LayoutElement>();
            }

            targetLayout.minHeight = sourceLayout.minHeight;
            targetLayout.preferredHeight = sourceLayout.preferredHeight;
            targetLayout.flexibleHeight = sourceLayout.flexibleHeight;
            targetLayout.minWidth = sourceLayout.minWidth;
            targetLayout.preferredWidth = sourceLayout.preferredWidth;
            targetLayout.flexibleWidth = sourceLayout.flexibleWidth;
        }

        private void ClearHost()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyInstance(transform.GetChild(i).gameObject);
            }
        }

        private static IDataBoundView<FeatureSectionData> ResolveBoundView(GameObject instance)
        {
            if (instance == null)
            {
                return null;
            }

            var components = instance.GetComponents<MonoBehaviour>();
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is IDataBoundView<FeatureSectionData> boundView)
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
