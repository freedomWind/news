using System;
using NewsFramework.Services.Article;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Content.Feed;
using NewsFramework.Services.Persistence.Sqlite;

namespace NewsFramework.Services.Content.Runtime
{
    public sealed class ContentRuntimeServices : IDisposable
    {
        public IContentService ContentService { get; set; }
        public IFeedPager FeedPager { get; set; }
        public IArticleEngagementService ArticleEngagementService { get; set; }
        public IContentCacheMaintenance CacheMaintenance { get; set; }
        public ISqliteDatabase Database { get; set; }

        public void Dispose()
        {
            if (Database is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Database = null;
            CacheMaintenance = null;
            ArticleEngagementService = null;
            ContentService = null;
            FeedPager = null;
        }
    }
}
