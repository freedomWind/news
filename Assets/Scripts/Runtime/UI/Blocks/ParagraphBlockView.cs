using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class ParagraphBlockView : BlockViewBase
    {
        private TextMeshProUGUI textLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("ParagraphBlock", parent);
            var view = root.gameObject.AddComponent<ParagraphBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 80f);

            view.textLabel = AppUIFactory.CreateText(
                "Text",
                root,
                string.Empty,
                16f,
                AppTheme.PrimaryText);
            AppUIFactory.Stretch(view.textLabel.rectTransform);
            view.textLabel.lineSpacing = 18f;
            view.textLabel.overflowMode = TextOverflowModes.Overflow;

            return view;
        }

        protected override void OnBind(BlockData data)
        {
            textLabel.text = data.text ?? string.Empty;
        }
    }
}
