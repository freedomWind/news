using System;
using System.Collections.Generic;
using System.IO;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Content.Feed;
using NewsFramework.Services.Content.Repositories;
using UnityEngine;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public static class SqliteSmokeTest
    {
        private const string DefaultFileName = "newsframework_sqlite_smoke.sqlite";

        public static SqliteSmokeTestResult Run(string databasePath = "")
        {
            var path = ResolvePath(databasePath);

            try
            {
                DeleteExistingDatabase(path);

                using (var database = new SqliteDatabase(path))
                {
                    RunRawSqlCheck(database);
                    RunContentRepositoryCheck(database);
                    RunFeedRepositoryCheck(database);
                    RunMaintenanceCheck(database);
                }

                return SqliteSmokeTestResult.Success(path, "SQLite smoke test passed.");
            }
            catch (Exception exception)
            {
                return SqliteSmokeTestResult.Failure(path, exception.GetType().Name + ": " + exception.Message);
            }
        }

        private static void RunMaintenanceCheck(ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            var maintenance = new SqliteContentCacheMaintenance(database, clock);
            var now = clock.UnixSeconds;

            database.Execute(
                "INSERT OR REPLACE INTO content_pages " +
                "(page_id, page_type, content_kind, version, json, fetched_at, expires_at, source, updated_at) " +
                "VALUES (@page_id, @page_type, @content_kind, @version, @json, @fetched_at, @expires_at, @source, @updated_at)",
                new SqliteParameter("@page_id", "expired_smoke_article"),
                new SqliteParameter("@page_type", ContentPageTypes.Article),
                new SqliteParameter("@content_kind", ContentKinds.Article),
                new SqliteParameter("@version", "expired_v1"),
                new SqliteParameter("@json", "{}"),
                new SqliteParameter("@fetched_at", now - 7200),
                new SqliteParameter("@expires_at", now - 3600),
                new SqliteParameter("@source", "smoke"),
                new SqliteParameter("@updated_at", now - 7200));

            database.Execute(
                "INSERT OR REPLACE INTO feed_pages " +
                "(feed_id, feed_version, cursor, next_cursor, title, blocks_json, has_more, estimated_total, " +
                "fetched_at, expires_at, source, updated_at) " +
                "VALUES (@feed_id, @feed_version, @cursor, @next_cursor, @title, @blocks_json, @has_more, " +
                "@estimated_total, @fetched_at, @expires_at, @source, @updated_at)",
                new SqliteParameter("@feed_id", "sqlite_smoke_feed"),
                new SqliteParameter("@feed_version", "feed_old"),
                new SqliteParameter("@cursor", "old_cursor"),
                new SqliteParameter("@next_cursor", string.Empty),
                new SqliteParameter("@title", "Old feed page"),
                new SqliteParameter("@blocks_json", "[]"),
                new SqliteParameter("@has_more", false),
                new SqliteParameter("@estimated_total", 0),
                new SqliteParameter("@fetched_at", now - 7200),
                new SqliteParameter("@expires_at", now - 3600),
                new SqliteParameter("@source", "smoke"),
                new SqliteParameter("@updated_at", now - 7200));

            ContentCacheMaintenanceResult result = null;
            maintenance.Cleanup(new ContentCacheMaintenancePolicy(), value => result = value);
            Require(result != null && result.success, "Maintenance cleanup failed.");
            Require(result.after.expiredContentPageCount == 0, "Expired content pages were not removed.");
            Require(result.after.expiredFeedPageCount == 0, "Expired feed pages were not removed.");

            var expiredRows = database.Query(
                "SELECT page_id FROM content_pages WHERE page_id = @page_id LIMIT 1",
                new SqliteParameter("@page_id", "expired_smoke_article"));
            Require(expiredRows.Count == 0, "Expired content page still exists.");
        }

        private static void RunRawSqlCheck(ISqliteDatabase database)
        {
            database.Execute(
                "CREATE TABLE IF NOT EXISTS smoke_values (" +
                "id TEXT PRIMARY KEY NOT NULL," +
                "text_value TEXT NOT NULL," +
                "int_value INTEGER NOT NULL" +
                ")");

            database.Execute("DELETE FROM smoke_values");
            database.Execute(
                "INSERT INTO smoke_values (id, text_value, int_value) VALUES (@id, @text_value, @int_value)",
                new SqliteParameter("@id", "raw"),
                new SqliteParameter("@text_value", "hello"),
                new SqliteParameter("@int_value", 7));

            var rows = database.Query(
                "SELECT text_value, int_value FROM smoke_values WHERE id = @id LIMIT 1",
                new SqliteParameter("@id", "raw"));

            Require(rows.Count == 1, "Raw SQL query returned no rows.");
            Require(rows[0]["text_value"] == "hello", "Raw SQL text value mismatch.");
            Require(rows[0].GetInt("int_value") == 7, "Raw SQL int value mismatch.");
        }

        private static void RunContentRepositoryCheck(ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            var repository = new SQLiteContentLocalRepository(database, clock);
            var now = clock.UnixSeconds;
            var entry = new ContentCacheEntry
            {
                page = new PageData
                {
                    pageId = "sqlite_smoke_article",
                    pageType = ContentPageTypes.Article,
                    title = "SQLite Smoke Article",
                    blocks = new List<BlockData>
                    {
                        new BlockData
                        {
                            id = "paragraph_1",
                            type = "paragraph",
                            text = "SQLite content repository smoke test."
                        }
                    }
                },
                metadata = new ContentCacheMetadata
                {
                    pageId = "sqlite_smoke_article",
                    pageType = ContentPageTypes.Article,
                    version = "content_v1",
                    fetchedAtUnixSeconds = now,
                    expiresAtUnixSeconds = now + 3600,
                    source = "smoke"
                }
            };

            var saved = false;
            repository.Save(entry, value => saved = value);
            Require(saved, "Content repository save failed.");

            ContentCacheEntry loaded = null;
            repository.Load(ContentRequest.Article("sqlite_smoke_article"), value => loaded = value);
            Require(loaded != null && loaded.HasPage, "Content repository load failed.");
            Require(loaded.metadata.version == "content_v1", "Content repository version mismatch.");
            Require(loaded.page.blocks.Count == 1, "Content repository block count mismatch.");
            Require(loaded.page.blocks[0].text == "SQLite content repository smoke test.", "Content repository block text mismatch.");
        }

        private static void RunFeedRepositoryCheck(ISqliteDatabase database)
        {
            var clock = new SystemContentClock();
            var repository = new SQLiteFeedPageLocalRepository(database, clock);
            var now = clock.UnixSeconds;
            var result = FeedPageResult.Success(
                "sqlite_smoke_feed",
                "SQLite Smoke Feed",
                "feed_v1",
                string.Empty,
                "cursor_2",
                true,
                2,
                new List<BlockData>
                {
                    new BlockData
                    {
                        id = "feed_1",
                        type = "news_item",
                        title = "SQLite feed smoke item",
                        text = "Feed page repository smoke test."
                    }
                },
                fetchedAtUnixSeconds: now,
                expiresAtUnixSeconds: now + 3600);

            var saved = false;
            repository.SavePage(result, value => saved = value);
            Require(saved, "Feed repository save failed.");

            FeedPageResult loaded = null;
            repository.LoadPage(
                new FeedPageRequest
                {
                    feedId = "sqlite_smoke_feed",
                    cursor = string.Empty,
                    knownVersion = "feed_v1",
                    limit = 6
                },
                value => loaded = value);

            Require(loaded != null && loaded.success, "Feed repository load failed.");
            Require(loaded.fromCache, "Feed repository result should be marked from cache.");
            Require(loaded.blocks.Count == 1, "Feed repository block count mismatch.");
            Require(loaded.blocks[0].title == "SQLite feed smoke item", "Feed repository block title mismatch.");
            Require(loaded.nextCursor == "cursor_2", "Feed repository next cursor mismatch.");
        }

        private static string ResolvePath(string databasePath)
        {
            if (!string.IsNullOrEmpty(databasePath))
            {
                return databasePath;
            }

            return Path.Combine(Application.temporaryCachePath, DefaultFileName);
        }

        private static void DeleteExistingDatabase(string databasePath)
        {
            if (string.IsNullOrEmpty(databasePath) || !File.Exists(databasePath))
            {
                return;
            }

            File.Delete(databasePath);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
