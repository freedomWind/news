using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content
{
    [Serializable]
    public sealed class ContentResult
    {
        public bool success;
        public string error;
        public PageData page;
        public ContentCacheMetadata metadata;
        public bool fromCache;
        public bool changed = true;

        public static ContentResult Success(
            PageData page,
            ContentCacheMetadata metadata = null,
            bool fromCache = false,
            bool changed = true)
        {
            return new ContentResult
            {
                success = true,
                page = page,
                metadata = metadata,
                fromCache = fromCache,
                changed = changed
            };
        }

        public static ContentResult Failure(string error)
        {
            return new ContentResult
            {
                success = false,
                error = error
            };
        }
    }
}
