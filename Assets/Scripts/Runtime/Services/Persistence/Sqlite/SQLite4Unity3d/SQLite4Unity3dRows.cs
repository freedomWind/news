using System.Collections.Generic;

namespace SQLite4Unity3d
{
    public partial class SQLiteCommand
    {
        public List<Dictionary<string, string>> ExecuteRows()
        {
            if (_conn.Trace)
            {
                _conn.InvokeTrace("Executing Raw Query: " + this);
            }

            var rows = new List<Dictionary<string, string>>();
            lock (_conn.SyncObject)
            {
                var statement = Prepare();
                try
                {
                    var columnCount = SQLite3.ColumnCount(statement);
                    var columnNames = new string[columnCount];
                    for (var i = 0; i < columnCount; i++)
                    {
                        columnNames[i] = SQLite3.ColumnName16(statement, i);
                    }

                    while (true)
                    {
                        var result = SQLite3.Step(statement);
                        if (result == SQLite3.Result.Done)
                        {
                            return rows;
                        }

                        if (result != SQLite3.Result.Row)
                        {
                            throw SQLiteException.New(result, SQLite3.GetErrmsg(_conn.Handle));
                        }

                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < columnCount; i++)
                        {
                            var value = SQLite3.ColumnType(statement, i) == SQLite3.ColType.Null
                                ? string.Empty
                                : SQLite3.ColumnString(statement, i);
                            row[columnNames[i]] = value;
                        }

                        rows.Add(row);
                    }
                }
                finally
                {
                    Finalize(statement);
                }
            }
        }
    }
}
