using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public sealed class EmptyStateSectionView : FeatureSectionViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("EmptyStateSection", parent);
            var view = root.gameObject.AddComponent<EmptyStateSectionView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 160f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 8f, new RectOffset(16, 16, 48, 24), TextAnchor.UpperCenter);

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root,
                string.Empty,
                18f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 28f);

            view.subtitleLabel = AppUIFactory.CreateText(
                "Subtitle",
                root,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(view.subtitleLabel.gameObject, 44f);
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            titleLabel.text = data.title ?? "页面建设中";
            subtitleLabel.text = data.subtitle ?? string.Empty;
        }
    }
}
