using System;

namespace NewsFramework.Services.Persistence.Sqlite
{
    [Serializable]
    public sealed class SqliteSmokeTestResult
    {
        public bool success;
        public string databasePath;
        public string message;

        public static SqliteSmokeTestResult Success(string databasePath, string message)
        {
            return new SqliteSmokeTestResult
            {
                success = true,
                databasePath = databasePath,
                message = message
            };
        }

        public static SqliteSmokeTestResult Failure(string databasePath, string message)
        {
            return new SqliteSmokeTestResult
            {
                success = false,
                databasePath = databasePath,
                message = message
            };
        }
    }
}
