using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class FeatureActionCardSectionView : FeatureSectionViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;
        private TextMeshProUGUI valueLabel;
        private TextMeshProUGUI actionLabel;
        private Button button;
        private FeatureItemData item;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = FeatureViewHelpers.CreateCardButton("FeatureActionCard", parent, null);
            var view = root.gameObject.AddComponent<FeatureActionCardSectionView>();
            view.button = root;
            AppUIFactory.AddLayoutElement(root.gameObject, 78f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 12f, new RectOffset(16, 16, 12, 12), TextAnchor.MiddleLeft);

            var textColumn = AppUIFactory.CreateRect("TextColumn", root.transform);
            var textLayout = AppUIFactory.AddLayoutElement(textColumn.gameObject, 54f);
            textLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(textColumn.gameObject, 4f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            view.titleLabel = FeatureViewHelpers.CreateTitle("Title", textColumn, string.Empty, 19f);
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 26f);
            view.subtitleLabel = FeatureViewHelpers.CreateMeta("Subtitle", textColumn, string.Empty);
            AppUIFactory.AddLayoutElement(view.subtitleLabel.gameObject, 20f);

            view.valueLabel = AppUIFactory.CreateText(
                "Value",
                root.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(view.valueLabel.gameObject, 54f, 80f);

            var actionButton = AppUIFactory.CreateImage("ActionPill", root.transform, AppTheme.SurfaceMuted);
            AppUIFactory.AddLayoutElement(actionButton.gameObject, 48f, 96f);
            view.actionLabel = AppUIFactory.CreateText(
                "Action",
                actionButton.transform,
                string.Empty,
                15f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.actionLabel.rectTransform);

            root.onClick.AddListener(() => view.TriggerItemAction(view.item));
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            item = data.items != null && data.items.Count > 0 ? data.items[0] : null;
            if (item == null)
            {
                return;
            }

            titleLabel.text = item.title ?? string.Empty;
            subtitleLabel.text = item.subtitle ?? string.Empty;
            valueLabel.text = item.value ?? string.Empty;
            actionLabel.text = string.IsNullOrEmpty(data.actionText) ? item.badge : data.actionText;
            button.interactable = FeatureViewHelpers.HasAction(item);
        }
    }
}
