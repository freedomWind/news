using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Persistence.Sqlite;
using UnityEngine;

namespace NewsFramework.Services.Content.Feed
{
    public sealed class SQLiteFeedPageLocalRepository : IFeedPageLocalRepository
    {
        private readonly ISqliteDatabase database;
        private readonly IContentClock clock;
        private readonly long defaultTtlSeconds;

        public SQLiteFeedPageLocalRepository(
            ISqliteDatabase database,
            IContentClock clock,
            long defaultTtlSeconds = 7200)
        {
            this.database = database;
            this.clock = clock ?? new SystemContentClock();
            this.defaultTtlSeconds = defaultTtlSeconds > 0 ? defaultTtlSeconds : 7200;
            SqliteContentSchema.Ensure(database);
        }

        public void LoadPage(FeedPageRequest request, Action<FeedPageResult> onComplete)
        {
            if (database == null || request == null || string.IsNullOrEmpty(request.feedId))
            {
                onComplete?.Invoke(null);
                return;
            }

            try
            {
                var cursor = request.cursor ?? string.Empty;
                var rows = database.Query(
                    "SELECT feed_id, feed_version, cursor, next_cursor, title, blocks_json, has_more, " +
                    "estimated_total, fetched_at, expires_at, source " +
                    "FROM feed_pages WHERE feed_id = @feed_id AND cursor = @cursor LIMIT 1",
                    new SqliteParameter("@feed_id", request.feedId),
                    new SqliteParameter("@cursor", cursor));

                if (rows.Count == 0)
                {
                    onComplete?.Invoke(null);
                    return;
                }

                var row = rows[0];
                if (!IsVersionUsable(request.knownVersion, row["feed_version"]))
                {
                    onComplete?.Invoke(null);
                    return;
                }

                var blocks = JsonConvert.DeserializeObject<List<BlockData>>(row["blocks_json"]) ?? new List<BlockData>();
                onComplete?.Invoke(FeedPageResult.Success(
                    row["feed_id"],
                    row["title"],
                    row["feed_version"],
                    row["cursor"],
                    row["next_cursor"],
                    row.GetBool("has_more"),
                    row.GetInt("estimated_total"),
                    blocks,
                    reset: cursor.Length == 0,
                    fromCache: true,
                    fetchedAtUnixSeconds: row.GetLong("fetched_at"),
                    expiresAtUnixSeconds: row.GetLong("expires_at")));
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite feed page load failed: " + exception.Message);
                onComplete?.Invoke(null);
            }
        }

        private static bool IsVersionUsable(string knownVersion, string cachedVersion)
        {
            return string.IsNullOrEmpty(knownVersion)
                || string.IsNullOrEmpty(cachedVersion)
                || knownVersion == cachedVersion;
        }

        public void SavePage(FeedPageResult result, Action<bool> onComplete)
        {
            if (database == null || result == null || string.IsNullOrEmpty(result.feedId))
            {
                onComplete?.Invoke(false);
                return;
            }

            try
            {
                var now = clock.UnixSeconds;
                var fetchedAt = result.fetchedAtUnixSeconds > 0 ? result.fetchedAtUnixSeconds : now;
                var expiresAt = result.expiresAtUnixSeconds > 0 ? result.expiresAtUnixSeconds : fetchedAt + defaultTtlSeconds;

                database.Execute(
                    "INSERT OR REPLACE INTO feed_pages " +
                    "(feed_id, feed_version, cursor, next_cursor, title, blocks_json, has_more, estimated_total, " +
                    "fetched_at, expires_at, source, updated_at) " +
                    "VALUES (@feed_id, @feed_version, @cursor, @next_cursor, @title, @blocks_json, @has_more, " +
                    "@estimated_total, @fetched_at, @expires_at, @source, @updated_at)",
                    new SqliteParameter("@feed_id", result.feedId),
                    new SqliteParameter("@feed_version", result.feedVersion),
                    new SqliteParameter("@cursor", result.cursor ?? string.Empty),
                    new SqliteParameter("@next_cursor", result.nextCursor),
                    new SqliteParameter("@title", result.title),
                    new SqliteParameter("@blocks_json", JsonConvert.SerializeObject(result.blocks ?? new List<BlockData>())),
                    new SqliteParameter("@has_more", result.hasMore),
                    new SqliteParameter("@estimated_total", result.estimatedTotal),
                    new SqliteParameter("@fetched_at", fetchedAt),
                    new SqliteParameter("@expires_at", expiresAt),
                    new SqliteParameter("@source", result.fromCache ? "sqlite" : "remote"),
                    new SqliteParameter("@updated_at", now));

                onComplete?.Invoke(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite feed page save failed: " + exception.Message);
                onComplete?.Invoke(false);
            }
        }

        public void ClearFeed(string feedId, Action<bool> onComplete)
        {
            if (database == null || string.IsNullOrEmpty(feedId))
            {
                onComplete?.Invoke(false);
                return;
            }

            try
            {
                database.Execute(
                    "DELETE FROM feed_pages WHERE feed_id = @feed_id",
                    new SqliteParameter("@feed_id", feedId));
                onComplete?.Invoke(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite feed clear failed: " + exception.Message);
                onComplete?.Invoke(false);
            }
        }
    }
}
