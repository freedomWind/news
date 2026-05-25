using System;
using System.Collections.Generic;

namespace NewsFramework.Data.Features
{
    [Serializable]
    public sealed class FeaturePageData
    {
        public string pageId;
        public string title;
        public List<FeatureSectionData> sections = new List<FeatureSectionData>();
    }
}
