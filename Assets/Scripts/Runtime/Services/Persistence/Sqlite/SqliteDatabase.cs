using System;
using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public sealed class SqliteDatabase : ISqliteDatabase, IDisposable
    {
        private readonly SQLiteConnection connection;
        private bool disposed;

        public SqliteDatabase(string databasePath)
        {
            if (string.IsNullOrEmpty(databasePath))
            {
                throw new SqliteException("SQLite database path is empty.");
            }

            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            connection = new SQLiteConnection(
                databasePath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        public void Execute(string sql, params SqliteParameter[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return;
            }

            EnsureOpen();
            var command = connection.CreateCommand(sql);
            BindParameters(command, parameters);
            command.ExecuteNonQuery();
        }

        public List<SqliteRow> Query(string sql, params SqliteParameter[] parameters)
        {
            var rows = new List<SqliteRow>();
            if (string.IsNullOrEmpty(sql))
            {
                return rows;
            }

            EnsureOpen();
            var command = connection.CreateCommand(sql);
            BindParameters(command, parameters);

            var rawRows = command.ExecuteRows();
            for (var i = 0; i < rawRows.Count; i++)
            {
                var row = new SqliteRow();
                foreach (var pair in rawRows[i])
                {
                    row.Set(pair.Key, pair.Value);
                }

                rows.Add(row);
            }

            return rows;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            connection?.Dispose();
        }

        private void EnsureOpen()
        {
            if (disposed)
            {
                throw new SqliteException("SQLite database is already disposed.");
            }
        }

        private static void BindParameters(SQLiteCommand command, SqliteParameter[] parameters)
        {
            if (command == null || parameters == null)
            {
                return;
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter == null || string.IsNullOrEmpty(parameter.name))
                {
                    continue;
                }

                command.Bind(parameter.name, parameter.value);
            }
        }
    }
}
