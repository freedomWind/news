using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class UnknownBlockView : BlockViewBase
    {
        private TextMeshProUGUI label;
        private string unknownType;

        public static UnknownBlockView Create(Transform parent)
        {
            var root = AppUIFactory.CreateImage("UnknownBlock", parent, AppTheme.SurfaceMuted);
            var view = root.gameObject.AddComponent<UnknownBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 48f);

            view.label = AppUIFactory.CreateText(
                "Label",
                root.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.label.rectTransform);
            return view;
        }

        public void SetUnknownType(string type)
        {
            unknownType = type;
        }

        protected override void OnBind(BlockData data)
        {
            label.text = "Unsupported block: " + (unknownType ?? data?.type ?? "empty");
        }
    }
}
