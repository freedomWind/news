using System;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content.Repositories
{
    public interface IContentLocalRepository
    {
        void Load(ContentRequest request, Action<ContentCacheEntry> onComplete);
        void Save(ContentCacheEntry entry, Action<bool> onComplete);
        void Remove(string pageId, Action<bool> onComplete);
    }
}
