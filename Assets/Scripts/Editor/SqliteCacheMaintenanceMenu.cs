using System;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Persistence.Sqlite;
using UnityEditor;
using UnityEngine;

namespace NewsFramework.Editor
{
    public static class SqliteCacheMaintenanceMenu
    {
        [MenuItem("NewsFramework/Diagnostics/Print SQLite Cache Stats")]
        public static void PrintSqliteCacheStats()
        {
            var databasePath = SqliteDatabaseFactory.ResolveDefaultPath();
            using (var database = new SqliteDatabase(databasePath))
            {
                var maintenance = new SqliteContentCacheMaintenance(database, new SystemContentClock());
                maintenance.GetStats(stats =>
                {
                    Debug.Log("SQLite cache stats: "
                        + FormatStats(stats)
                        + " Database: "
                        + databasePath);
                });
            }
        }

        [MenuItem("NewsFramework/Diagnostics/Cleanup SQLite Cache")]
        public static void CleanupSqliteCache()
        {
            var databasePath = SqliteDatabaseFactory.ResolveDefaultPath();
            using (var database = new SqliteDatabase(databasePath))
            {
                var maintenance = new SqliteContentCacheMaintenance(database, new SystemContentClock());
                maintenance.Cleanup(new ContentCacheMaintenancePolicy(), result =>
                {
                    if (result == null || !result.success)
                    {
                        var error = result != null ? result.error : "empty result";
                        Debug.LogError("SQLite cache cleanup failed: " + error + " Database: " + databasePath);
                        throw new InvalidOperationException(error);
                    }

                    Debug.Log("SQLite cache cleanup passed. Removed content="
                        + result.RemovedContentPages
                        + ", feed="
                        + result.RemovedFeedPages
                        + ". Before: "
                        + FormatStats(result.before)
                        + ". After: "
                        + FormatStats(result.after)
                        + ". Database: "
                        + databasePath);
                });
            }
        }

        private static string FormatStats(ContentCacheStats stats)
        {
            if (stats == null)
            {
                return "empty";
            }

            return "content="
                + stats.contentPageCount
                + " expiredContent="
                + stats.expiredContentPageCount
                + " feedPages="
                + stats.feedPageCount
                + " expiredFeedPages="
                + stats.expiredFeedPageCount
                + " feeds="
                + stats.feedCount;
        }
    }
}
