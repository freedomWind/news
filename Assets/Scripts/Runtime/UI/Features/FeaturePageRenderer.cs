using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public sealed class FeaturePageRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform contentRoot;

        private FeatureSectionRegistry registry;
        private Action<BlockActionData> onAction;

        public void Initialize(
            RectTransform root,
            FeatureSectionRegistry sectionRegistry,
            Action<BlockActionData> actionHandler)
        {
            contentRoot = root;
            registry = sectionRegistry;
            onAction = actionHandler;
        }

        public void Render(FeaturePageData page)
        {
            Clear();

            if (page == null || page.sections == null || contentRoot == null)
            {
                return;
            }

            if (registry == null)
            {
                registry = FeatureSectionRegistry.CreateDefault();
            }

            for (var i = 0; i < page.sections.Count; i++)
            {
                var section = page.sections[i];
                if (section == null)
                {
                    continue;
                }

                var view = registry.Create(section, contentRoot);
                view.Bind(section, onAction);
            }
        }

        public void Clear()
        {
            if (contentRoot == null)
            {
                return;
            }

            for (var i = contentRoot.childCount - 1; i >= 0; i--)
            {
                var child = contentRoot.GetChild(i);
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

        public void AddSpacer(float height)
        {
            if (height <= 0f || contentRoot == null)
            {
                return;
            }

            var spacer = AppUIFactory.CreateRect("FeatureSpacer", contentRoot);
            AppUIFactory.AddLayoutElement(spacer.gameObject, height);
        }
    }
}
