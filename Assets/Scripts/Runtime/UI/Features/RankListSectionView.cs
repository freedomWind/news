using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class RankListSectionView : FeatureSectionViewBase
    {
        private Transform rowRoot;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = FeatureViewHelpers.CreateCard("RankList", parent);
            var view = root.gameObject.AddComponent<RankListSectionView>();
            view.rowRoot = root.transform;
            AppUIFactory.AddVerticalLayout(root.gameObject, 0f, new RectOffset(0, 0, 0, 0));
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            var count = data.items != null ? data.items.Count : 0;
            AppUIFactory.AddLayoutElement(gameObject, Mathf.Max(1, count) * 64f);

            for (var i = 0; i < count; i++)
            {
                CreateRankRow(data.items[i]);
            }
        }

        private void CreateRankRow(FeatureItemData item)
        {
            var row = AppUIFactory.CreateRect("Rank_" + item.id, rowRoot);
            AppUIFactory.AddLayoutElement(row.gameObject, 64f);
            AppUIFactory.AddHorizontalLayout(row.gameObject, 10f, new RectOffset(14, 14, 8, 8), TextAnchor.MiddleLeft);

            var iconBg = AppUIFactory.CreateImage("IconBg", row, item.state == "dark" ? AppTheme.PrimaryText : AppTheme.Accent);
            AppUIFactory.AddLayoutElement(iconBg.gameObject, 34f, 34f);
            var icon = AppUIFactory.CreateText("Icon", iconBg.transform, string.IsNullOrEmpty(item.icon) ? "棋" : item.icon, 17f, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(icon.rectTransform);

            var column = AppUIFactory.CreateRect("TextColumn", row);
            var columnLayout = AppUIFactory.AddLayoutElement(column.gameObject, 48f);
            columnLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(column.gameObject, 2f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);
            var title = FeatureViewHelpers.CreateTitle("Title", column, item.title ?? string.Empty, 16f);
            AppUIFactory.AddLayoutElement(title.gameObject, 24f);
            var subtitle = FeatureViewHelpers.CreateMeta("Subtitle", column, item.subtitle ?? string.Empty);
            AppUIFactory.AddLayoutElement(subtitle.gameObject, 18f);

            var rank = AppUIFactory.CreateText("Rank", row, item.value ?? string.Empty, 16f, AppTheme.Rgb(200, 164, 92), FontStyles.Bold, TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(rank.gameObject, 34f, 82f);
        }
    }
}
