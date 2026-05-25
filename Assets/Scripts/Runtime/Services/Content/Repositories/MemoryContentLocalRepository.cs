using System;
using System.Collections.Generic;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content.Repositories
{
    public sealed class MemoryContentLocalRepository : IContentLocalRepository
    {
        private readonly Dictionary<string, ContentCacheEntry> entries = new Dictionary<string, ContentCacheEntry>();

        public void Load(ContentRequest request, Action<ContentCacheEntry> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.pageId))
            {
                onComplete?.Invoke(null);
                return;
            }

            entries.TryGetValue(request.pageId, out var entry);
            onComplete?.Invoke(entry);
        }

        public void Save(ContentCacheEntry entry, Action<bool> onComplete)
        {
            if (entry == null || entry.page == null || string.IsNullOrEmpty(entry.page.pageId))
            {
                onComplete?.Invoke(false);
                return;
            }

            entries[entry.page.pageId] = entry;
            onComplete?.Invoke(true);
        }

        public void Remove(string pageId, Action<bool> onComplete)
        {
            var removed = !string.IsNullOrEmpty(pageId) && entries.Remove(pageId);
            onComplete?.Invoke(removed);
        }
    }
}
