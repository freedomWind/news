namespace NewsFramework.Services.Persistence.Sqlite
{
    public sealed class SqliteParameter
    {
        public string name;
        public object value;

        public SqliteParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
