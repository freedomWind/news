using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class SectionTitleBlockView : BlockViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("SectionTitleBlock", parent);
            var view = root.gameObject.AddComponent<SectionTitleBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 38f);

            var marker = AppUIFactory.CreateImage("Marker", root, AppTheme.Accent);
            marker.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            marker.rectTransform.anchorMax = new Vector2(0f, 0.5f);
            marker.rectTransform.pivot = new Vector2(0f, 0.5f);
            marker.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            marker.rectTransform.sizeDelta = new Vector2(4f, 21f);

            var row = AppUIFactory.CreateRect("TitleRow", root);
            row.anchorMin = Vector2.zero;
            row.anchorMax = Vector2.one;
            row.offsetMin = new Vector2(14f, 0f);
            row.offsetMax = Vector2.zero;
            AppUIFactory.AddHorizontalLayout(row.gameObject, 6f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                row,
                string.Empty,
                21f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.MidlineLeft);
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 32f);

            view.subtitleLabel = AppUIFactory.CreateText(
                "Subtitle",
                row,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft);
            var subtitleLayout = AppUIFactory.AddLayoutElement(view.subtitleLabel.gameObject, 28f);
            subtitleLayout.flexibleWidth = 1f;

            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = data.text ?? data.title ?? string.Empty;
            subtitleLabel.text = data.subtitle ?? string.Empty;
        }
    }
}
