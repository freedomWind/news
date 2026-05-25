using NewsFramework.Services.Article;
using NewsFramework.Services.Article.Repositories;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Content.Feed;
using NewsFramework.Services.Content.Http;
using NewsFramework.Services.Content.Repositories;
using NewsFramework.Services.Content.Serialization;
using NewsFramework.Services.Persistence.Sqlite;

namespace NewsFramework.Services.Content
{
    public static class ContentServiceFactory
    {
        public static CachedContentService CreateMockCachedService()
        {
            var clock = new SystemContentClock();
            var policy = new ContentCachePolicy();
            var memoryStore = new MemoryContentStore();
            var localRepository = new MemoryContentLocalRepository();
            var remoteRepository = new MockContentRemoteRepository(policy, clock);
            return new CachedContentService(memoryStore, localRepository, remoteRepository, clock);
        }

        public static IFeedPager CreateMockFeedPager()
        {
            return new FeedPager(new MockFeedPageRemoteRepository());
        }

        public static IArticleEngagementService CreateMockArticleEngagementService()
        {
            return new MockArticleEngagementService();
        }

        public static IArticleEngagementService CreateHttpArticleEngagementService(ContentHttpConfig httpConfig)
        {
            return new RemoteArticleEngagementService(new HttpArticleEngagementRemoteRepository(
                httpConfig,
                CreateHttpClient(httpConfig),
                new NewtonsoftContentJsonSerializer()));
        }

        public static IFeedPager CreateSQLiteMockFeedPager(string databasePath = "")
        {
            return CreateSQLiteMockFeedPager(SqliteDatabaseFactory.CreateDefault(databasePath));
        }

        public static IFeedPager CreateSQLiteMockFeedPager(ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            return new FeedPager(
                new MockFeedPageRemoteRepository(),
                new SQLiteFeedPageLocalRepository(database, clock),
                clock);
        }

        public static IFeedPager CreateHttpFeedPager(ContentHttpConfig httpConfig)
        {
            return new FeedPager(new HttpFeedPageRemoteRepository(
                httpConfig,
                CreateHttpClient(httpConfig),
                new NewtonsoftContentJsonSerializer()));
        }

        public static IFeedPager CreateSQLiteFeedPager(ContentHttpConfig httpConfig, string databasePath = "")
        {
            return CreateSQLiteFeedPager(httpConfig, SqliteDatabaseFactory.CreateDefault(databasePath));
        }

        public static IFeedPager CreateSQLiteFeedPager(ContentHttpConfig httpConfig, ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            return new FeedPager(
                new HttpFeedPageRemoteRepository(
                    httpConfig,
                    CreateHttpClient(httpConfig),
                    new NewtonsoftContentJsonSerializer()),
                new SQLiteFeedPageLocalRepository(database, clock),
                clock);
        }

        public static CachedContentService CreateHttpCachedService(ContentHttpConfig httpConfig)
        {
            var clock = new SystemContentClock();
            var policy = new ContentCachePolicy();
            var memoryStore = new MemoryContentStore();
            var localRepository = new MemoryContentLocalRepository();
            var remoteRepository = new HttpContentRemoteRepository(
                httpConfig,
                CreateHttpClient(httpConfig),
                new NewtonsoftContentJsonSerializer(),
                policy,
                clock);

            return new CachedContentService(memoryStore, localRepository, remoteRepository, clock);
        }

        public static CachedContentService CreateSQLiteCachedService(ContentHttpConfig httpConfig, string databasePath = "")
        {
            return CreateSQLiteCachedService(httpConfig, SqliteDatabaseFactory.CreateDefault(databasePath));
        }

        public static CachedContentService CreateSQLiteCachedService(ContentHttpConfig httpConfig, ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            var policy = new ContentCachePolicy();
            var memoryStore = new MemoryContentStore();
            var localRepository = new SQLiteContentLocalRepository(database, clock);
            var remoteRepository = new HttpContentRemoteRepository(
                httpConfig,
                CreateHttpClient(httpConfig),
                new NewtonsoftContentJsonSerializer(),
                policy,
                clock);

            return new CachedContentService(memoryStore, localRepository, remoteRepository, clock);
        }

        public static CachedContentService CreateSQLiteMockCachedService(string databasePath = "")
        {
            return CreateSQLiteMockCachedService(SqliteDatabaseFactory.CreateDefault(databasePath));
        }

        public static CachedContentService CreateSQLiteMockCachedService(ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            var policy = new ContentCachePolicy();
            var memoryStore = new MemoryContentStore();
            var localRepository = new SQLiteContentLocalRepository(database, clock);
            var remoteRepository = new MockContentRemoteRepository(policy, clock);

            return new CachedContentService(memoryStore, localRepository, remoteRepository, clock);
        }

        private static IContentHttpClient CreateHttpClient(ContentHttpConfig httpConfig)
        {
            var safeConfig = httpConfig ?? new ContentHttpConfig();
            return new RetryingContentHttpClient(
                new UnityWebRequestContentHttpClient(),
                safeConfig.maxRetryCount);
        }
    }
}
