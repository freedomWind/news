using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class SpacerBlockView : BlockViewBase
    {
        private LayoutElement layoutElement;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("SpacerBlock", parent);
            var view = root.gameObject.AddComponent<SpacerBlockView>();
            view.layoutElement = AppUIFactory.AddLayoutElement(root.gameObject, 12f);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            layoutElement.preferredHeight = Mathf.Max(0f, data.height);
        }
    }
}
