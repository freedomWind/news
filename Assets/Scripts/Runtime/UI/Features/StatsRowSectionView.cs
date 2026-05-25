using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class StatsRowSectionView : FeatureSectionViewBase
    {
        private Transform rowRoot;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("StatsRow", parent);
            var view = root.gameObject.AddComponent<StatsRowSectionView>();
            view.rowRoot = root;
            AppUIFactory.AddLayoutElement(root.gameObject, 72f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            if (data.items == null)
            {
                return;
            }

            for (var i = 0; i < data.items.Count; i++)
            {
                CreateStat(data.items[i]);
            }
        }

        private void CreateStat(FeatureItemData item)
        {
            var card = FeatureViewHelpers.CreateCard("Stat_" + item.id, rowRoot);
            var layout = AppUIFactory.AddLayoutElement(card.gameObject, 72f);
            layout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(card.gameObject, 2f, new RectOffset(6, 6, 8, 6), TextAnchor.MiddleCenter);

            var value = AppUIFactory.CreateText("Value", card.transform, item.value ?? string.Empty, 26f, AppTheme.PrimaryText, FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(value.gameObject, 34f);
            var title = FeatureViewHelpers.CreateMeta("Title", card.transform, item.title ?? string.Empty, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(title.gameObject, 20f);
        }
    }
}
