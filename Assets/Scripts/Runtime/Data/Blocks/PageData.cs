using System;
using System.Collections.Generic;

namespace NewsFramework.Data.Blocks
{
    [Serializable]
    public sealed class PageData
    {
        public string pageId;
        public string pageType;
        public string title;
        public List<BlockData> blocks = new List<BlockData>();
    }
}
