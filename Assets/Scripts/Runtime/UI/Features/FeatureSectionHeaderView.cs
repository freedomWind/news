using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class FeatureSectionHeaderView : FeatureSectionViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI actionLabel;
        private Button actionButton;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("FeatureSectionHeader", parent);
            var view = root.gameObject.AddComponent<FeatureSectionHeaderView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 30f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            var bar = AppUIFactory.CreateImage("Accent", root, AppTheme.Accent);
            AppUIFactory.AddLayoutElement(bar.gameObject, 20f, 3f);

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root,
                string.Empty,
                20f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.MidlineLeft);
            var titleLayout = AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 28f);
            titleLayout.flexibleWidth = 1f;

            view.actionButton = AppUIFactory.CreateButton(
                "ActionButton",
                root,
                AppTheme.PageBackground,
                view.TriggerSectionAction);
            AppUIFactory.AddLayoutElement(view.actionButton.gameObject, 28f, 96f);

            view.actionLabel = AppUIFactory.CreateText(
                "Label",
                view.actionButton.transform,
                string.Empty,
                13f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.MidlineRight);
            AppUIFactory.Stretch(view.actionLabel.rectTransform);
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            titleLabel.text = data.title ?? string.Empty;
            actionLabel.text = data.actionText ?? string.Empty;
            actionButton.gameObject.SetActive(!string.IsNullOrEmpty(data.actionText));
        }
    }
}
