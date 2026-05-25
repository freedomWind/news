using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public sealed class UnknownFeatureSectionView : FeatureSectionViewBase
    {
        private string unknownType;
        private TextMeshProUGUI label;

        public static UnknownFeatureSectionView Create(Transform parent, string type)
        {
            var root = AppUIFactory.CreateRect("UnknownFeatureSection", parent);
            var view = root.gameObject.AddComponent<UnknownFeatureSectionView>();
            view.unknownType = type;
            AppUIFactory.AddLayoutElement(root.gameObject, 48f);
            view.label = AppUIFactory.CreateText(
                "Label",
                root,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.label.rectTransform);
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            label.text = "未知功能区块: " + unknownType;
        }
    }
}
