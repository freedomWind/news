namespace NewsFramework.Services.Persistence.Sqlite
{
    public static class SqliteContentSchema
    {
        public static void Ensure(ISqliteDatabase database)
        {
            if (database == null)
            {
                return;
            }

            database.Execute(
                "CREATE TABLE IF NOT EXISTS content_pages (" +
                "page_id TEXT PRIMARY KEY NOT NULL," +
                "page_type TEXT NOT NULL," +
                "content_kind TEXT NOT NULL," +
                "version TEXT," +
                "json TEXT NOT NULL," +
                "fetched_at INTEGER NOT NULL," +
                "expires_at INTEGER NOT NULL," +
                "source TEXT," +
                "updated_at INTEGER NOT NULL" +
                ")");

            database.Execute(
                "CREATE INDEX IF NOT EXISTS idx_content_pages_expires_at " +
                "ON content_pages(expires_at)");

            database.Execute(
                "CREATE INDEX IF NOT EXISTS idx_content_pages_updated_at " +
                "ON content_pages(updated_at)");

            database.Execute(
                "CREATE TABLE IF NOT EXISTS feed_pages (" +
                "feed_id TEXT NOT NULL," +
                "feed_version TEXT," +
                "cursor TEXT NOT NULL," +
                "next_cursor TEXT," +
                "title TEXT," +
                "blocks_json TEXT NOT NULL," +
                "has_more INTEGER NOT NULL," +
                "estimated_total INTEGER NOT NULL," +
                "fetched_at INTEGER NOT NULL," +
                "expires_at INTEGER NOT NULL," +
                "source TEXT," +
                "updated_at INTEGER NOT NULL," +
                "PRIMARY KEY(feed_id, cursor)" +
                ")");

            database.Execute(
                "CREATE INDEX IF NOT EXISTS idx_feed_pages_feed_updated " +
                "ON feed_pages(feed_id, updated_at)");

            database.Execute(
                "CREATE INDEX IF NOT EXISTS idx_feed_pages_expires_at " +
                "ON feed_pages(expires_at)");

            database.Execute(
                "CREATE INDEX IF NOT EXISTS idx_feed_pages_feed_version " +
                "ON feed_pages(feed_id, feed_version)");
        }
    }
}
