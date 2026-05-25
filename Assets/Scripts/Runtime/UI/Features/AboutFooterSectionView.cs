using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public sealed class AboutFooterSectionView : FeatureSectionViewBase
    {
        private Transform rootTransform;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("AboutFooter", parent);
            var view = root.gameObject.AddComponent<AboutFooterSectionView>();
            view.rootTransform = root;
            AppUIFactory.AddVerticalLayout(root.gameObject, 10f, new RectOffset(4, 4, 6, 18), TextAnchor.UpperLeft);
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            var count = data.items != null ? data.items.Count : 0;
            AppUIFactory.AddLayoutElement(gameObject, count * 30f + 72f);

            for (var i = 0; i < count; i++)
            {
                var item = data.items[i];
                var row = AppUIFactory.CreateText("FooterLink_" + item.id, rootTransform, item.title ?? string.Empty, 14f, AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.Left);
                AppUIFactory.AddLayoutElement(row.gameObject, 24f);
            }

            var version = AppUIFactory.CreateText("Version", rootTransform, data.subtitle ?? "App版本 v1.0.0", 12f, AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(version.gameObject, 22f);

            var slogan = AppUIFactory.CreateText("Slogan", rootTransform, data.title ?? "为棋友用心打造", 13f, AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(slogan.gameObject, 22f);
        }
    }
}
