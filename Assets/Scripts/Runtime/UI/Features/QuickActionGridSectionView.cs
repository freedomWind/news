using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class QuickActionGridSectionView : FeatureSectionViewBase
    {
        private Transform cardRoot;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("QuickActionGrid", parent);
            var view = root.gameObject.AddComponent<QuickActionGridSectionView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 150f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 12f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);
            view.cardRoot = root;
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            if (data.items == null)
            {
                return;
            }

            for (var i = 0; i < data.items.Count; i++)
            {
                CreateCard(data.items[i]);
            }
        }

        private void CreateCard(FeatureItemData item)
        {
            var button = FeatureViewHelpers.CreateCardButton("QuickAction_" + item.id, cardRoot, () => TriggerItemAction(item));
            var layout = AppUIFactory.AddLayoutElement(button.gameObject, 146f);
            layout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(button.gameObject, 7f, new RectOffset(12, 12, 12, 10), TextAnchor.UpperCenter);

            var icon = AppUIFactory.CreateText(
                "Icon",
                button.transform,
                string.IsNullOrEmpty(item.icon) ? "棋" : item.icon,
                30f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(icon.gameObject, 38f);

            var title = AppUIFactory.CreateText(
                "Title",
                button.transform,
                item.title ?? string.Empty,
                18f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            title.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(title.gameObject, 24f);

            var subtitle = FeatureViewHelpers.CreateMeta("Subtitle", button.transform, item.subtitle ?? string.Empty, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(subtitle.gameObject, 20f);

            var badgeRoot = AppUIFactory.CreateImage(
                "Badge",
                button.transform,
                string.IsNullOrEmpty(item.badge) ? AppTheme.Surface : AppTheme.Accent);
            AppUIFactory.AddLayoutElement(badgeRoot.gameObject, 22f, 118f);
            var badge = AppUIFactory.CreateText(
                "Label",
                badgeRoot.transform,
                item.badge ?? string.Empty,
                11f,
                Color.white,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(badge.rectTransform);
            badgeRoot.gameObject.SetActive(!string.IsNullOrEmpty(item.badge));
        }
    }
}
