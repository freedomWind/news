using System;
using NewsFramework.Data.Blocks;

namespace NewsFramework.Services.Content.Cache
{
    [Serializable]
    public sealed class ContentCacheEntry
    {
        public PageData page;
        public ContentCacheMetadata metadata = new ContentCacheMetadata();

        public bool HasPage => page != null && !string.IsNullOrEmpty(page.pageId);
    }
}
