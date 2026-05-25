using System.Collections.Generic;
using NewsFramework.Data.Features;

namespace NewsFramework.Data.Mock
{
    public static class FeatureMockData
    {
        public static FeaturePageData CreateDataPage()
        {
            return new FeaturePageData
            {
                pageId = "data",
                title = "数据",
                sections = new List<FeatureSectionData>
                {
                    new FeatureSectionData
                    {
                        id = "data_empty",
                        type = "empty_state",
                        title = "数据模块建设中",
                        subtitle = "后续排行榜、棋谱库和个人统计页会接入同一套功能页渲染协议。"
                    }
                }
            };
        }
    }
}
