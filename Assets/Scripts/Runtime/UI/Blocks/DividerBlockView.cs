using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class DividerBlockView : BlockViewBase
    {
        public static BlockViewBase Create(Transform parent)
        {
            var image = AppUIFactory.CreateImage("DividerBlock", parent, AppTheme.Hairline);
            var view = image.gameObject.AddComponent<DividerBlockView>();
            AppUIFactory.AddLayoutElement(image.gameObject, 1f);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
        }
    }
}
