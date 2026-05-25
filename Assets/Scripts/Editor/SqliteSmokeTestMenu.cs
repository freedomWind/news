using System;
using System.IO;
using NewsFramework.Services.Persistence.Sqlite;
using UnityEditor;
using UnityEngine;

namespace NewsFramework.Editor
{
    public static class SqliteSmokeTestMenu
    {
        [MenuItem("NewsFramework/Diagnostics/Run SQLite Smoke Test")]
        public static void RunSqliteSmokeTest()
        {
            var databasePath = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "../Temp/SqliteSmoke/newsframework_sqlite_smoke.sqlite"));
            var result = SqliteSmokeTest.Run(databasePath);
            var message = result.message + " Database: " + result.databasePath;

            if (!result.success)
            {
                Debug.LogError(message);
                throw new InvalidOperationException(message);
            }

            Debug.Log(message);
        }
    }
}
