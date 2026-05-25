namespace NewsFramework.Services.Content.Serialization
{
    public interface IContentJsonSerializer
    {
        T FromJson<T>(string json) where T : class;
    }
}
