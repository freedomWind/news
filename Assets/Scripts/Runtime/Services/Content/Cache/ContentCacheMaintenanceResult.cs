using System;

namespace NewsFramework.Services.Content.Cache
{
    [Serializable]
    public sealed class ContentCacheMaintenanceResult
    {
        public bool success;
        public string error;
        public ContentCacheStats before = new ContentCacheStats();
        public ContentCacheStats after = new ContentCacheStats();

        public int RemovedContentPages => Math.Max(0, before.contentPageCount - after.contentPageCount);
        public int RemovedFeedPages => Math.Max(0, before.feedPageCount - after.feedPageCount);

        public static ContentCacheMaintenanceResult Success(ContentCacheStats before, ContentCacheStats after)
        {
            return new ContentCacheMaintenanceResult
            {
                success = true,
                before = before ?? new ContentCacheStats(),
                after = after ?? new ContentCacheStats()
            };
        }

        public static ContentCacheMaintenanceResult Failure(string error, ContentCacheStats before = null)
        {
            return new ContentCacheMaintenanceResult
            {
                success = false,
                error = error,
                before = before ?? new ContentCacheStats(),
                after = before ?? new ContentCacheStats()
            };
        }
    }
}
