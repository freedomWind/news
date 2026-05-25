using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class RecentMatchListSectionView : FeatureSectionViewBase
    {
        private Transform rowRoot;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = FeatureViewHelpers.CreateCard("RecentMatchList", parent);
            var view = root.gameObject.AddComponent<RecentMatchListSectionView>();
            view.rowRoot = root.transform;
            AppUIFactory.AddVerticalLayout(root.gameObject, 0f, new RectOffset(14, 14, 0, 0));
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            var count = data.items != null ? data.items.Count : 0;
            AppUIFactory.AddLayoutElement(gameObject, Mathf.Max(1, count) * 56f);

            for (var i = 0; i < count; i++)
            {
                CreateRow(data.items[i], i < count - 1);
            }
        }

        private void CreateRow(FeatureItemData item, bool divider)
        {
            var row = AppUIFactory.CreateRect("RecentMatch_" + item.id, rowRoot);
            AppUIFactory.AddLayoutElement(row.gameObject, 56f);
            AppUIFactory.AddHorizontalLayout(row.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            var icon = AppUIFactory.CreateText("Icon", row, "□", 18f, AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(icon.gameObject, 24f, 24f);

            var title = FeatureViewHelpers.CreateTitle("Title", row, item.title ?? string.Empty, 14f);
            var titleLayout = AppUIFactory.AddLayoutElement(title.gameObject, 28f);
            titleLayout.flexibleWidth = 1f;

            var result = AppUIFactory.CreateText(
                "Result",
                row,
                item.result ?? string.Empty,
                11f,
                FeatureViewHelpers.ResultColor(item.result),
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(result.gameObject, 22f, 30f);

            var time = FeatureViewHelpers.CreateMeta("Time", row, item.time ?? string.Empty, TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(time.gameObject, 28f, 64f);

            if (!divider)
            {
                return;
            }

            var line = AppUIFactory.CreateImage("Divider", row, AppTheme.Hairline);
            var lineLayout = line.gameObject.AddComponent<LayoutElement>();
            lineLayout.ignoreLayout = true;
            line.rectTransform.anchorMin = Vector2.zero;
            line.rectTransform.anchorMax = new Vector2(1f, 0f);
            line.rectTransform.pivot = new Vector2(0.5f, 0f);
            line.rectTransform.offsetMin = new Vector2(32f, 0f);
            line.rectTransform.offsetMax = Vector2.zero;
        }
    }
}
