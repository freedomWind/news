using System;
using NewsFramework.Data.Blocks;

namespace NewsFramework.Data.Features
{
    [Serializable]
    public sealed class FeatureItemData
    {
        public string id;
        public string title;
        public string subtitle;
        public string value;
        public string detail;
        public string badge;
        public string state;
        public string icon;
        public string time;
        public string result;
        public bool locked;
        public BlockActionData action = BlockActionData.None();
    }
}
