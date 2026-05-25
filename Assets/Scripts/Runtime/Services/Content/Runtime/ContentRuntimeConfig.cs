using System;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Content.Feed;
using NewsFramework.Services.Content.Http;

namespace NewsFramework.Services.Content.Runtime
{
    [Serializable]
    public sealed class ContentRuntimeConfig
    {
        public ContentRuntimeMode mode = ContentRuntimeMode.Mock;
        public ContentHttpConfig httpConfig = new ContentHttpConfig();
        public string sqliteDatabasePath;
        public bool runCacheMaintenanceOnStart = true;
        public ContentCacheMaintenancePolicy cacheMaintenancePolicy = new ContentCacheMaintenancePolicy();
        public string feedId = "home";
        public int feedPageSize = 6;
        public float feedPrefetchScreens = 2.5f;
        public bool feedActiveOnStart = true;

        public FeedPagerConfig CreateFeedPagerConfig()
        {
            return new FeedPagerConfig
            {
                feedId = string.IsNullOrEmpty(feedId) ? "home" : feedId,
                pageSize = Math.Max(1, feedPageSize),
                prefetchScreens = Math.Max(0.5f, feedPrefetchScreens),
                activeOnStart = feedActiveOnStart
            };
        }
    }
}
