using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class ArticleTipCardBlockView : BlockViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI summaryLabel;
        private TextMeshProUGUI badgeLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("ArticleTipCardBlock", parent, AppTheme.Surface, null);
            var view = root.gameObject.AddComponent<ArticleTipCardBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 118f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 8f, new RectOffset(14, 14, 12, 10));

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root.transform,
                string.Empty,
                19f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            view.titleLabel.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 28f);

            view.summaryLabel = AppUIFactory.CreateText(
                "Summary",
                root.transform,
                string.Empty,
                14f,
                AppTheme.SecondaryText);
            view.summaryLabel.lineSpacing = 2f;
            view.summaryLabel.maxVisibleLines = 2;
            AppUIFactory.AddLayoutElement(view.summaryLabel.gameObject, 44f);

            var footer = AppUIFactory.CreateRect("Footer", root.transform);
            AppUIFactory.AddLayoutElement(footer.gameObject, 22f);

            view.badgeLabel = AppUIFactory.CreateText(
                "Badge",
                footer,
                string.Empty,
                12f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineRight);
            view.badgeLabel.rectTransform.anchorMin = Vector2.zero;
            view.badgeLabel.rectTransform.anchorMax = Vector2.one;
            view.badgeLabel.rectTransform.offsetMin = Vector2.zero;
            view.badgeLabel.rectTransform.offsetMax = Vector2.zero;

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = data.title ?? string.Empty;
            summaryLabel.text = data.text ?? data.subtitle ?? string.Empty;
            badgeLabel.text = string.IsNullOrEmpty(data.badge) ? string.Empty : data.badge;
        }
    }
}
