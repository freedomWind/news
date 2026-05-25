using System;
using NewsFramework.Data.Blocks;

namespace NewsFramework.UI.Rendering
{
    public interface IDataBoundView<in TData>
    {
        void Bind(TData data, Action<BlockActionData> onAction);
    }
}
