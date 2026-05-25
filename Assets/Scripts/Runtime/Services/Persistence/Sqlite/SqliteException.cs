using System;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public sealed class SqliteException : Exception
    {
        public SqliteException(string message) : base(message)
        {
        }
    }
}
