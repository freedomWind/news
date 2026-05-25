using System;
using NewsFramework.Data.Blocks;

namespace NewsFramework.UI.Blocks
{
    public interface IBlockView
    {
        void Bind(BlockData data, Action<BlockActionData> onAction);
    }
}
