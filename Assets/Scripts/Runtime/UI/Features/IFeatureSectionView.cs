using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;

namespace NewsFramework.UI.Features
{
    public interface IFeatureSectionView
    {
        void Bind(FeatureSectionData data, Action<BlockActionData> onAction);
    }
}
