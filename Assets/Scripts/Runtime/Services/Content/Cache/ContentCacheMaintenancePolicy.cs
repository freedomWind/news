using System;

namespace NewsFramework.Services.Content.Cache
{
    [Serializable]
    public sealed class ContentCacheMaintenancePolicy
    {
        public bool removeExpiredEntries = true;
        public bool removeOlderFeedVersions = true;
        public int maxContentPages = 200;
        public int maxFeedPagesPerFeed = 30;
    }
}
