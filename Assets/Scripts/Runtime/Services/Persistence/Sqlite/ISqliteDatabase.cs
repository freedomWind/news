using System.Collections.Generic;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public interface ISqliteDatabase
    {
        void Execute(string sql, params SqliteParameter[] parameters);
        List<SqliteRow> Query(string sql, params SqliteParameter[] parameters);
    }
}
