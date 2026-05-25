using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class AchievementStripSectionView : FeatureSectionViewBase
    {
        private Transform rowRoot;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("AchievementStrip", parent);
            var view = root.gameObject.AddComponent<AchievementStripSectionView>();
            view.rowRoot = root;
            AppUIFactory.AddLayoutElement(root.gameObject, 102f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 12f, new RectOffset(0, 0, 2, 2), TextAnchor.MiddleLeft);
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
                CreateAchievement(data.items[i]);
            }
        }

        private void CreateAchievement(FeatureItemData item)
        {
            var root = AppUIFactory.CreateRect("Achievement_" + item.id, rowRoot);
            AppUIFactory.AddLayoutElement(root.gameObject, 96f, 72f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 5f, new RectOffset(0, 0, 0, 0), TextAnchor.UpperCenter);

            var badge = AppUIFactory.CreateImage("Badge", root, item.locked ? AppTheme.SurfaceMuted : AppTheme.Rgb(200, 164, 92));
            AppUIFactory.AddLayoutElement(badge.gameObject, 62f, 54f);
            var icon = AppUIFactory.CreateText("Icon", badge.transform, string.IsNullOrEmpty(item.icon) ? "奖" : item.icon, 22f, item.locked ? AppTheme.SecondaryText : Color.white, FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(icon.rectTransform);

            var label = AppUIFactory.CreateText("Label", root, item.title ?? string.Empty, 11f, item.locked ? AppTheme.SecondaryText : AppTheme.PrimaryText, FontStyles.Bold, TextAlignmentOptions.Center);
            label.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(label.gameObject, 20f);
        }
    }
}
