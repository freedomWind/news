using System;
using NewsFramework.Data.Mock;

namespace NewsFramework.Services.Content
{
    public sealed class MockContentService : IContentService
    {
        private readonly IContentStore store;

        public event Action<ContentResult> PageUpdated
        {
            add { }
            remove { }
        }

        public MockContentService(IContentStore store)
        {
            this.store = store;
        }

        public void LoadPage(ContentRequest request, Action<ContentResult> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.pageId))
            {
                onComplete?.Invoke(ContentResult.Failure("Content request is empty."));
                return;
            }

            if (store != null && store.TryGetPage(request.pageId, out var cached))
            {
                onComplete?.Invoke(ContentResult.Success(cached));
                return;
            }

            RefreshPage(request, onComplete);
        }

        public void RefreshPage(ContentRequest request, Action<ContentResult> onComplete)
        {
            var page = CreateMockPage(request);
            if (page == null)
            {
                onComplete?.Invoke(ContentResult.Failure("Unknown mock page: " + request?.pageId));
                return;
            }

            store?.SetPage(page);
            onComplete?.Invoke(ContentResult.Success(page));
        }

        private static NewsFramework.Data.Blocks.PageData CreateMockPage(ContentRequest request)
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
    }
}
