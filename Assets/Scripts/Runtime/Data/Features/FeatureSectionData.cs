using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;

namespace NewsFramework.Data.Features
{
    [Serializable]
    public sealed class FeatureSectionData
    {
        public string id;
        public string type;
        public string title;
        public string subtitle;
        public string actionText;
        public BlockActionData action = BlockActionData.None();
        public List<FeatureItemData> items = new List<FeatureItemData>();
    }
}
