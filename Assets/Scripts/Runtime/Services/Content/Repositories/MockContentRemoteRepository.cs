using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Mock;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content.Repositories
{
    public sealed class MockContentRemoteRepository : IContentRemoteRepository
    {
        private readonly ContentCachePolicy cachePolicy;
        private readonly IContentClock clock;

        public MockContentRemoteRepository(ContentCachePolicy cachePolicy, IContentClock clock)
        {
            this.cachePolicy = cachePolicy ?? new ContentCachePolicy();
            this.clock = clock ?? new SystemContentClock();
        }

        public void Fetch(ContentRequest request, Action<ContentCacheEntry> onComplete)
        {
            var page = CreateMockPage(request);
            if (page == null)
            {
                onComplete?.Invoke(null);
                
                return;
            }

            var now = clock.UnixSeconds;
            var pageType = ResolvePageType(request, page);
            var ttl = cachePolicy.ResolveTtlSeconds(pageType);
            var version = ResolveVersion(request);

            onComplete?.Invoke(new ContentCacheEntry
            {
                page = page,
                metadata = new ContentCacheMetadata
                {
                    pageId = page.pageId,
                    pageType = pageType,
                    version = version,
                    fetchedAtUnixSeconds = now,
                    expiresAtUnixSeconds = now + ttl,
                    source = "mock_remote"
                }
            });
        }

        private static PageData CreateMockPage(ContentRequest request)
        {
            if (request == null)
            {
                return null;
            }

            switch (request.pageId)
            {
                case "article_001":
                case "article_002":
                    return ArticleMockData.Create(request.pageId);
                default:
                    return null;
            }
        }

        private static string ResolvePageType(ContentRequest request, PageData page)
        {
            if (request != null && !string.IsNullOrEmpty(request.pageType))
            {
                return request.pageType;
            }

            return page != null && !string.IsNullOrEmpty(page.pageType)
                ? page.pageType
                : ContentPageTypes.Page;
        }

        private static string ResolveVersion(ContentRequest request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var serverVersion = request.GetParameter("serverVersion");
            if (!string.IsNullOrEmpty(serverVersion))
            {
                return serverVersion;
            }

            return request.pageId + "_v1";
        }
    }
}
