using System.Collections.Generic;

namespace NewsFramework.Services.Persistence.Sqlite
{
    public sealed class SqliteRow
    {
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        public string this[string key] => values.TryGetValue(key, out var value) ? value : string.Empty;

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            values[key] = value;
        }

        public long GetLong(string key)
        {
            return long.TryParse(this[key], out var value) ? value : 0L;
        }

        public int GetInt(string key)
        {
            return int.TryParse(this[key], out var value) ? value : 0;
        }

        public bool GetBool(string key)
        {
            return GetInt(key) != 0;
        }
    }
}
