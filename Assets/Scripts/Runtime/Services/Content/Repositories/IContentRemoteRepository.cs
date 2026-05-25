using System;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content.Repositories
{
    public interface IContentRemoteRepository
    {
        void Fetch(ContentRequest request, Action<ContentCacheEntry> onComplete);
    }
}
