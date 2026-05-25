using System;
using Newtonsoft.Json;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Persistence.Sqlite;
using UnityEngine;

namespace NewsFramework.Services.Content.Repositories
{
    public sealed class SQLiteContentLocalRepository : IContentLocalRepository
    {
        private readonly ISqliteDatabase database;
        private readonly IContentClock clock;

        public SQLiteContentLocalRepository(ISqliteDatabase database, IContentClock clock)
        {
            this.database = database;
            this.clock = clock ?? new SystemContentClock();
            SqliteContentSchema.Ensure(database);
        }

        public void Load(ContentRequest request, Action<ContentCacheEntry> onComplete)
        {
            if (database == null || request == null || string.IsNullOrEmpty(request.pageId))
            {
                onComplete?.Invoke(null);
                return;
            }

            try
            {
                var rows = database.Query(
                    "SELECT page_id, page_type, version, json, fetched_at, expires_at, source " +
                    "FROM content_pages WHERE page_id = @page_id LIMIT 1",
                    new SqliteParameter("@page_id", request.pageId));

                if (rows.Count == 0)
                {
                    onComplete?.Invoke(null);
                    return;
                }

                var row = rows[0];
                var page = JsonConvert.DeserializeObject<PageData>(row["json"]);
                if (page == null)
                {
                    onComplete?.Invoke(null);
                    return;
                }

                onComplete?.Invoke(new ContentCacheEntry
                {
                    page = page,
                    metadata = new ContentCacheMetadata
                    {
                        pageId = row["page_id"],
                        pageType = row["page_type"],
                        version = row["version"],
                        fetchedAtUnixSeconds = row.GetLong("fetched_at"),
                        expiresAtUnixSeconds = row.GetLong("expires_at"),
                        source = row["source"]
                    }
                });
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite content load failed: " + exception.Message);
                onComplete?.Invoke(null);
            }
        }

        public void Save(ContentCacheEntry entry, Action<bool> onComplete)
        {
            if (database == null || entry == null || entry.page == null || string.IsNullOrEmpty(entry.page.pageId))
            {
                onComplete?.Invoke(false);
                return;
            }

            try
            {
                var metadata = entry.metadata ?? new ContentCacheMetadata();
                var pageType = !string.IsNullOrEmpty(metadata.pageType) ? metadata.pageType : entry.page.pageType;
                if (string.IsNullOrEmpty(pageType))
                {
                    pageType = Cache.ContentPageTypes.Page;
                }

                database.Execute(
                    "INSERT OR REPLACE INTO content_pages " +
                    "(page_id, page_type, content_kind, version, json, fetched_at, expires_at, source, updated_at) " +
                    "VALUES (@page_id, @page_type, @content_kind, @version, @json, @fetched_at, @expires_at, @source, @updated_at)",
                    new SqliteParameter("@page_id", entry.page.pageId),
                    new SqliteParameter("@page_type", pageType),
                    new SqliteParameter("@content_kind", ResolveContentKind(pageType)),
                    new SqliteParameter("@version", metadata.version),
                    new SqliteParameter("@json", JsonConvert.SerializeObject(entry.page)),
                    new SqliteParameter("@fetched_at", metadata.fetchedAtUnixSeconds),
                    new SqliteParameter("@expires_at", metadata.expiresAtUnixSeconds),
                    new SqliteParameter("@source", metadata.source),
                    new SqliteParameter("@updated_at", clock.UnixSeconds));

                onComplete?.Invoke(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite content save failed: " + exception.Message);
                onComplete?.Invoke(false);
            }
        }

        public void Remove(string pageId, Action<bool> onComplete)
        {
            if (database == null || string.IsNullOrEmpty(pageId))
            {
                onComplete?.Invoke(false);
                return;
            }

            try
            {
                database.Execute(
                    "DELETE FROM content_pages WHERE page_id = @page_id",
                    new SqliteParameter("@page_id", pageId));
                onComplete?.Invoke(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("SQLite content remove failed: " + exception.Message);
                onComplete?.Invoke(false);
            }
        }

        private static string ResolveContentKind(string pageType)
        {
            switch (pageType)
            {
                case Cache.ContentPageTypes.Article:
                    return ContentKinds.Article;
                case Cache.ContentPageTypes.Replay:
                    return ContentKinds.Replay;
                default:
                    return ContentKinds.Page;
            }
        }
    }
}
