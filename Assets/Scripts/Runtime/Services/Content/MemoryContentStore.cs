using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content
{
    public sealed class MemoryContentStore : IContentStore
    {
        private readonly Dictionary<string, ContentCacheEntry> entries = new Dictionary<string, ContentCacheEntry>();

        public bool TryGetEntry(string pageId, out ContentCacheEntry entry)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                entry = null;
                return false;
            }

            return entries.TryGetValue(pageId, out entry);
        }

        public void SetEntry(ContentCacheEntry entry)
        {
            if (entry == null || entry.page == null || string.IsNullOrEmpty(entry.page.pageId))
            {
                return;
            }

            entries[entry.page.pageId] = entry;
        }

        public bool TryGetPage(string pageId, out PageData page)
        {
            if (!TryGetEntry(pageId, out var entry) || entry == null)
            {
                page = null;
                return false;
            }

            page = entry.page;
            return page != null;
        }

        public void SetPage(PageData page)
        {
            if (page == null || string.IsNullOrEmpty(page.pageId))
            {
                return;
            }

            SetEntry(new ContentCacheEntry
            {
                page = page,
                metadata = new ContentCacheMetadata
                {
                    pageId = page.pageId,
                    pageType = page.pageType,
                    source = "memory"
                }
            });
        }

        public void RemovePage(string pageId)
        {
            if (!string.IsNullOrEmpty(pageId))
            {
                entries.Remove(pageId);
            }
        }

        public void Clear()
        {
            entries.Clear();
        }
    }
}
