using Newtonsoft.Json;

namespace NewsFramework.Services.Content.Serialization
{
    public sealed class NewtonsoftContentJsonSerializer : IContentJsonSerializer
    {
        public T FromJson<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
