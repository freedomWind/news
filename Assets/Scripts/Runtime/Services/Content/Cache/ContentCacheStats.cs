using System;

namespace NewsFramework.Services.Content.Cache
{
    [Serializable]
    public sealed class ContentCacheStats
    {
        public int contentPageCount;
        public int expiredContentPageCount;
        public int feedPageCount;
        public int expiredFeedPageCount;
        public int feedCount;

        public int TotalRows => contentPageCount + feedPageCount;
        public int TotalExpiredRows => expiredContentPageCount + expiredFeedPageCount;
    }
}
