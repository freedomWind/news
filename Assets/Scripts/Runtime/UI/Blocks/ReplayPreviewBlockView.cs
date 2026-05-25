using NewsFramework.Data.Blocks;
using NewsFramework.Data.Replay;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class ReplayPreviewBlockView : BlockViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;
        private TextMeshProUGUI stepLabel;
        private TextMeshProUGUI boardLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("ReplayPreviewBlock", parent, AppTheme.Surface, null);
            var view = root.gameObject.AddComponent<ReplayPreviewBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 258f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 10f, new RectOffset(16, 16, 14, 14));

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root.transform,
                string.Empty,
                18f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            view.titleLabel.maxVisibleLines = 1;

            view.subtitleLabel = AppUIFactory.CreateText(
                "Subtitle",
                root.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal);
            view.subtitleLabel.maxVisibleLines = 1;

            var surface = AppUIFactory.CreateImage("PreviewSurface", root.transform, AppTheme.SurfaceMuted);
            AppUIFactory.AddLayoutElement(surface.gameObject, 142f);

            var icon = AppUIFactory.CreateText(
                "Icon",
                surface.transform,
                "♞",
                42f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            icon.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            icon.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            icon.rectTransform.anchoredPosition = new Vector2(0f, 14f);
            icon.rectTransform.sizeDelta = new Vector2(80f, 54f);

            view.boardLabel = AppUIFactory.CreateText(
                "BoardLabel",
                surface.transform,
                string.Empty,
                12f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            view.boardLabel.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            view.boardLabel.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            view.boardLabel.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            view.boardLabel.rectTransform.anchoredPosition = new Vector2(0f, -28f);
            view.boardLabel.rectTransform.sizeDelta = new Vector2(0f, 28f);

            var footer = AppUIFactory.CreateRect("Footer", root.transform);
            AppUIFactory.AddLayoutElement(footer.gameObject, 26f);
            AppUIFactory.AddHorizontalLayout(footer.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            view.stepLabel = AppUIFactory.CreateText(
                "Step",
                footer,
                string.Empty,
                12f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft);
            var stepLayout = AppUIFactory.AddLayoutElement(view.stepLabel.gameObject, 24f);
            stepLayout.flexibleWidth = 1f;

            var actionLabel = AppUIFactory.CreateText(
                "Action",
                footer,
                "查看回放 >",
                12f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(actionLabel.gameObject, 24f, 82f);

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = string.IsNullOrEmpty(data.title) ? "关键回放" : data.title;
            subtitleLabel.text = string.IsNullOrEmpty(data.subtitle) ? "点击查看完整复盘" : data.subtitle;
            boardLabel.text = BuildBoardLabel(data);
            stepLabel.text = BuildStepLabel(data.replay);
        }

        private static string BuildBoardLabel(BlockData data)
        {
            if (!string.IsNullOrEmpty(data.boardTitle))
            {
                return data.boardTitle;
            }

            if (data.replay != null && !string.IsNullOrEmpty(data.replay.initialState))
            {
                return "局面预览";
            }

            return "回放预览";
        }

        private static string BuildStepLabel(ReplayData replay)
        {
            if (replay == null)
            {
                return "回放摘要";
            }

            if (replay.steps != null && replay.steps.Count > 0)
            {
                var first = replay.steps[0];
                var notation = string.IsNullOrEmpty(first.notation) ? first.command : first.notation;
                return string.IsNullOrEmpty(notation)
                    ? "第 " + first.index + " 回合"
                    : "第 " + first.index + " 回合 - " + notation;
            }

            return "第 " + replay.startStepIndex + " - " + replay.ResolveEndStepIndex() + " 回合";
        }
    }
}
