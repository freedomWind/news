using System;
using NewsFramework.Data.Blocks;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public abstract class BlockViewBase : MonoBehaviour, IBlockView
    {
        protected BlockData Data { get; private set; }
        protected Action<BlockActionData> OnAction { get; private set; }
        protected BlockRenderContext Context { get; private set; }

        public void SetContext(BlockRenderContext context)
        {
            Context = context ?? BlockRenderContext.CreateDefault();
        }

        public void Bind(BlockData data, Action<BlockActionData> onAction)
        {
            if (Context == null)
            {
                Context = BlockRenderContext.CreateDefault();
            }

            Data = data;
            OnAction = onAction;
            OnBind(data);
        }

        protected abstract void OnBind(BlockData data);

        protected void TriggerAction()
        {
            if (Data == null || Data.action == null || Data.action.type == "none")
            {
                return;
            }

            OnAction?.Invoke(Data.action);
        }
    }
}
