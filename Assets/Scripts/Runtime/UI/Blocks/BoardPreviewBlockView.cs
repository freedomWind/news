using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class BoardPreviewBlockView : BlockViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI boardTitleLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("BoardPreviewBlock", parent, AppTheme.Surface, null);
            var view = root.gameObject.AddComponent<BoardPreviewBlockView>();

            AppUIFactory.AddLayoutElement(root.gameObject, 250f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 10f, new RectOffset(16, 16, 14, 14));

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root.transform,
                string.Empty,
                18f,
                AppTheme.PrimaryText,
                FontStyles.Bold);

            var board = AppUIFactory.CreateImage("BoardSurface", root.transform, AppTheme.SurfaceMuted);
            AppUIFactory.AddLayoutElement(board.gameObject, 174f);

            var icon = AppUIFactory.CreateText(
                "BoardIcon",
                board.transform,
                "♜",
                42f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            icon.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            icon.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            icon.rectTransform.anchoredPosition = new Vector2(0f, 16f);
            icon.rectTransform.sizeDelta = new Vector2(80f, 56f);

            view.boardTitleLabel = AppUIFactory.CreateText(
                "BoardTitle",
                board.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            view.boardTitleLabel.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            view.boardTitleLabel.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            view.boardTitleLabel.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            view.boardTitleLabel.rectTransform.anchoredPosition = new Vector2(0f, -26f);
            view.boardTitleLabel.rectTransform.sizeDelta = new Vector2(0f, 28f);

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = data.title ?? "棋局预览";
            boardTitleLabel.text = data.boardTitle ?? "局面预览";
        }
    }
}
