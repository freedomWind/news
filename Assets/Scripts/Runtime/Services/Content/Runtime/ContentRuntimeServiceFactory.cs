using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Persistence.Sqlite;
using UnityEngine;

namespace NewsFramework.Services.Content.Runtime
{
    public static class ContentRuntimeServiceFactory
    {
        public static ContentRuntimeServices Create(ContentRuntimeConfig config)
        {
            var safeConfig = config ?? new ContentRuntimeConfig();
            switch (safeConfig.mode)
            {
                case ContentRuntimeMode.Http:
                    return CreateHttp(safeConfig);
                case ContentRuntimeMode.SQLiteMock:
                    return CreateSQLiteMock(safeConfig);
                case ContentRuntimeMode.SQLiteHttp:
                    return CreateSQLiteHttp(safeConfig);
                default:
                    return CreateMock();
            }
        }

        private static ContentRuntimeServices CreateMock()
        {
            return new ContentRuntimeServices
            {
                ContentService = ContentServiceFactory.CreateMockCachedService(),
                FeedPager = ContentServiceFactory.CreateMockFeedPager(),
                ArticleEngagementService = ContentServiceFactory.CreateMockArticleEngagementService()
            };
        }

        private static ContentRuntimeServices CreateHttp(ContentRuntimeConfig config)
        {
            return new ContentRuntimeServices
            {
                ContentService = ContentServiceFactory.CreateHttpCachedService(config.httpConfig),
                FeedPager = ContentServiceFactory.CreateHttpFeedPager(config.httpConfig),
                ArticleEngagementService = ContentServiceFactory.CreateHttpArticleEngagementService(config.httpConfig)
            };
        }

        private static ContentRuntimeServices CreateSQLiteMock(ContentRuntimeConfig config)
        {
            var database = SqliteDatabaseFactory.CreateDefault(config.sqliteDatabasePath);
            var clock = new SystemContentClock();
            var services = new ContentRuntimeServices
            {
                Database = database,
                CacheMaintenance = new SqliteContentCacheMaintenance(database, clock),
                ContentService = ContentServiceFactory.CreateSQLiteMockCachedService(database),
                FeedPager = ContentServiceFactory.CreateSQLiteMockFeedPager(database),
                ArticleEngagementService = ContentServiceFactory.CreateMockArticleEngagementService()
            };
            RunStartupMaintenanceIfNeeded(services, config);
            return services;
        }

        private static ContentRuntimeServices CreateSQLiteHttp(ContentRuntimeConfig config)
        {
            var database = SqliteDatabaseFactory.CreateDefault(config.sqliteDatabasePath);
            var clock = new SystemContentClock();
            var services = new ContentRuntimeServices
            {
                Database = database,
                CacheMaintenance = new SqliteContentCacheMaintenance(database, clock),
                ContentService = ContentServiceFactory.CreateSQLiteCachedService(config.httpConfig, database),
                FeedPager = ContentServiceFactory.CreateSQLiteFeedPager(config.httpConfig, database),
                ArticleEngagementService = ContentServiceFactory.CreateHttpArticleEngagementService(config.httpConfig)
            };
            RunStartupMaintenanceIfNeeded(services, config);
            return services;
        }

        private static void RunStartupMaintenanceIfNeeded(ContentRuntimeServices services, ContentRuntimeConfig config)
        {
            if (services == null || services.CacheMaintenance == null || config == null || !config.runCacheMaintenanceOnStart)
            {
                return;
            }

            services.CacheMaintenance.Cleanup(config.cacheMaintenancePolicy, result =>
            {
                if (result == null || !result.success)
                {
                    Debug.LogWarning("SQLite startup cache maintenance failed: " + (result != null ? result.error : "empty result"));
                    return;
                }

                if (result.RemovedContentPages > 0 || result.RemovedFeedPages > 0)
                {
                    Debug.Log("SQLite startup cache maintenance removed content="
                        + result.RemovedContentPages
                        + ", feed="
                        + result.RemovedFeedPages);
                }
            });
        }
    }
}
