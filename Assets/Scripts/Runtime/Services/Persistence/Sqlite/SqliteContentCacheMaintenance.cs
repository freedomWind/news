using System;
using System.Collections.Generic;
using NewsFramework.Services.Content.Cache;
using UnityEngine;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public sealed class SqliteContentCacheMaintenance : IContentCacheMaintenance
    {
        private readonly ISqliteDatabase database;
        private readonly IContentClock clock;

        public SqliteContentCacheMaintenance(ISqliteDatabase database, IContentClock clock)
        {
            this.database = database;
            this.clock = clock ?? new SystemContentClock();
            SqliteContentSchema.Ensure(database);
        }

        public void GetStats(Action<ContentCacheStats> onComplete)
        {
            if (database == null)
            {
                onComplete?.Invoke(new ContentCacheStats());
                return;
            }

            try
            {
                onComplete?.Invoke(ReadStats());
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite cache stats failed: " + exception.Message);
                onComplete?.Invoke(new ContentCacheStats());
            }
        }

        public void Cleanup(ContentCacheMaintenancePolicy policy, Action<ContentCacheMaintenanceResult> onComplete)
        {
            if (database == null)
            {
                onComplete?.Invoke(ContentCacheMaintenanceResult.Failure("SQLite database is missing."));
                return;
            }

            var safePolicy = policy ?? new ContentCacheMaintenancePolicy();
            ContentCacheStats before = null;
            try
            {
                before = ReadStats();

                if (safePolicy.removeExpiredEntries)
                {
                    RemoveExpiredEntries(clock.UnixSeconds);
                }

                if (safePolicy.removeOlderFeedVersions)
                {
                    RemoveOlderFeedVersions();
                }

                TrimContentPages(safePolicy.maxContentPages);
                TrimFeedPagesPerFeed(safePolicy.maxFeedPagesPerFeed);

                onComplete?.Invoke(ContentCacheMaintenanceResult.Success(before, ReadStats()));
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite cache cleanup failed: " + exception.Message);
                onComplete?.Invoke(ContentCacheMaintenanceResult.Failure(exception.Message, before));
            }
        }

        private ContentCacheStats ReadStats()
        {
            var now = clock.UnixSeconds;
            return new ContentCacheStats
            {
                contentPageCount = Count("SELECT COUNT(*) AS count FROM content_pages"),
                expiredContentPageCount = Count(
                    "SELECT COUNT(*) AS count FROM content_pages WHERE expires_at > 0 AND expires_at <= @now",
                    new SqliteParameter("@now", now)),
                feedPageCount = Count("SELECT COUNT(*) AS count FROM feed_pages"),
                expiredFeedPageCount = Count(
                    "SELECT COUNT(*) AS count FROM feed_pages WHERE expires_at > 0 AND expires_at <= @now",
                    new SqliteParameter("@now", now)),
                feedCount = Count("SELECT COUNT(DISTINCT feed_id) AS count FROM feed_pages")
            };
        }

        private int Count(string sql, params SqliteParameter[] parameters)
        {
            var rows = database.Query(sql, parameters);
            return rows.Count == 0 ? 0 : rows[0].GetInt("count");
        }

        private void RemoveExpiredEntries(long now)
        {
            database.Execute(
                "DELETE FROM content_pages WHERE expires_at > 0 AND expires_at <= @now",
                new SqliteParameter("@now", now));
            database.Execute(
                "DELETE FROM feed_pages WHERE expires_at > 0 AND expires_at <= @now",
                new SqliteParameter("@now", now));
        }

        private void RemoveOlderFeedVersions()
        {
            var feedIds = ReadColumn(
                "SELECT DISTINCT feed_id FROM feed_pages WHERE feed_id <> ''",
                "feed_id");

            for (var i = 0; i < feedIds.Count; i++)
            {
                var feedId = feedIds[i];
                var versionRows = database.Query(
                    "SELECT feed_version FROM feed_pages " +
                    "WHERE feed_id = @feed_id AND feed_version <> '' " +
                    "ORDER BY updated_at DESC LIMIT 1",
                    new SqliteParameter("@feed_id", feedId));

                if (versionRows.Count == 0)
                {
                    continue;
                }

                var latestVersion = versionRows[0]["feed_version"];
                if (string.IsNullOrEmpty(latestVersion))
                {
                    continue;
                }

                database.Execute(
                    "DELETE FROM feed_pages " +
                    "WHERE feed_id = @feed_id AND feed_version <> '' AND feed_version <> @feed_version",
                    new SqliteParameter("@feed_id", feedId),
                    new SqliteParameter("@feed_version", latestVersion));
            }
        }

        private void TrimContentPages(int maxContentPages)
        {
            if (maxContentPages <= 0)
            {
                return;
            }

            var pageIds = ReadColumn(
                "SELECT page_id FROM content_pages ORDER BY updated_at DESC",
                "page_id");

            for (var i = maxContentPages; i < pageIds.Count; i++)
            {
                database.Execute(
                    "DELETE FROM content_pages WHERE page_id = @page_id",
                    new SqliteParameter("@page_id", pageIds[i]));
            }
        }

        private void TrimFeedPagesPerFeed(int maxFeedPagesPerFeed)
        {
            if (maxFeedPagesPerFeed <= 0)
            {
                return;
            }

            var feedIds = ReadColumn(
                "SELECT DISTINCT feed_id FROM feed_pages WHERE feed_id <> ''",
                "feed_id");

            for (var i = 0; i < feedIds.Count; i++)
            {
                var feedId = feedIds[i];
                var cursors = ReadColumn(
                    "SELECT cursor FROM feed_pages WHERE feed_id = @feed_id ORDER BY updated_at DESC",
                    "cursor",
                    new SqliteParameter("@feed_id", feedId));

                for (var cursorIndex = maxFeedPagesPerFeed; cursorIndex < cursors.Count; cursorIndex++)
                {
                    database.Execute(
                        "DELETE FROM feed_pages WHERE feed_id = @feed_id AND cursor = @cursor",
                        new SqliteParameter("@feed_id", feedId),
                        new SqliteParameter("@cursor", cursors[cursorIndex]));
                }
            }
        }

        private List<string> ReadColumn(string sql, string columnName, params SqliteParameter[] parameters)
        {
            var values = new List<string>();
            var rows = database.Query(sql, parameters);
            for (var i = 0; i < rows.Count; i++)
            {
                values.Add(rows[i][columnName]);
            }

            return values;
        }
    }
}
