using System.IO;
using UnityEngine;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public static class SqliteDatabaseFactory
    {
        public const string DefaultDatabaseName = "newsframework_content.sqlite";

        public static string ResolveDefaultPath(string databasePath = "")
        {
            return string.IsNullOrEmpty(databasePath)
                ? Path.Combine(Application.persistentDataPath, DefaultDatabaseName)
                : databasePath;
        }

        public static ISqliteDatabase CreateDefault(string databasePath = "")
        {
            return new SqliteDatabase(ResolveDefaultPath(databasePath));
        }
    }
}
