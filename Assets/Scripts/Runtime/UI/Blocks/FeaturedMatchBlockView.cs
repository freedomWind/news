using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class FeaturedMatchBlockView : BlockViewBase
    {
        private TextMeshProUGUI badgeLabel;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;
        private TextMeshProUGUI boardTitleLabel;
        private TextMeshProUGUI sourceLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("FeaturedMatchBlock", parent, AppTheme.Surface, null);
            var view = root.gameObject.AddComponent<FeaturedMatchBlockView>();
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 386f);

            AppUIFactory.AddLayoutElement(root.gameObject, 386f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 12f, new RectOffset(16, 16, 16, 14));

            var badge = AppUIFactory.CreateText(
                "Badge",
                root.transform,
                string.Empty,
                13f,
                AppTheme.Accent,
                FontStyles.Bold);
            view.badgeLabel = badge;

            var title = AppUIFactory.CreateText(
                "Title",
                root.transform,
                string.Empty,
                23f,
                AppTheme.PrimaryText,
                FontStyles.Normal);
            title.overflowMode = TextOverflowModes.Ellipsis;
            title.maxVisibleLines = 1;
            view.titleLabel = title;

            var subtitle = AppUIFactory.CreateText(
                "Subtitle",
                root.transform,
                string.Empty,
                15f,
                AppTheme.SecondaryText);
            subtitle.maxVisibleLines = 1;
            view.subtitleLabel = subtitle;

            var boardPanel = AppUIFactory.CreateImage("BoardPreviewPanel", root.transform, AppTheme.SurfaceMuted);
            AppUIFactory.AddLayoutElement(boardPanel.gameObject, 192f);

            var boardRect = boardPanel.rectTransform;
            var boardIcon = AppUIFactory.CreateText(
                "BoardIcon",
                boardPanel.transform,
                "♞",
                44f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            var boardIconRect = boardIcon.rectTransform;
            boardIconRect.anchorMin = new Vector2(0.5f, 0.5f);
            boardIconRect.anchorMax = new Vector2(0.5f, 0.5f);
            boardIconRect.pivot = new Vector2(0.5f, 0.5f);
            boardIconRect.anchoredPosition = new Vector2(0f, 16f);
            boardIconRect.sizeDelta = new Vector2(80f, 56f);

            var boardTitle = AppUIFactory.CreateText(
                "BoardTitle",
                boardPanel.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            var boardTitleRect = boardTitle.rectTransform;
            boardTitleRect.anchorMin = new Vector2(0f, 0.5f);
            boardTitleRect.anchorMax = new Vector2(1f, 0.5f);
            boardTitleRect.pivot = new Vector2(0.5f, 0.5f);
            boardTitleRect.anchoredPosition = new Vector2(0f, -26f);
            boardTitleRect.sizeDelta = new Vector2(0f, 28f);
            view.boardTitleLabel = boardTitle;

            var footer = AppUIFactory.CreateRect("Footer", root.transform);
            AppUIFactory.AddLayoutElement(footer.gameObject, 34f);
            AppUIFactory.AddHorizontalLayout(footer.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            var source = AppUIFactory.CreateText(
                "Source",
                footer,
                string.Empty,
                13f,
                AppTheme.SecondaryText);
            source.text = "♟  中国象棋";
            AppUIFactory.AddLayoutElement(source.gameObject, 24f);
            var sourceLayout = source.GetComponent<LayoutElement>();
            sourceLayout.flexibleWidth = 1f;
            view.sourceLabel = source;

            var detail = AppUIFactory.CreateText(
                "Detail",
                footer,
                "查看详情 >",
                13f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.Right);
            AppUIFactory.AddLayoutElement(detail.gameObject, -1f, 96f);

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            badgeLabel.text = string.IsNullOrEmpty(data.badge) ? string.Empty : "♨  " + data.badge;
            titleLabel.text = data.title ?? string.Empty;
            subtitleLabel.text = data.subtitle ?? string.Empty;
            boardTitleLabel.text = data.boardTitle ?? data.title ?? string.Empty;
            sourceLabel.text = string.IsNullOrEmpty(data.source) ? "♟" : "♟  " + data.source;
        }
    }
}
