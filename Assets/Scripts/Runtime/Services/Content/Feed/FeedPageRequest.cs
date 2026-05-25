using System;
using System.Collections.Generic;

namespace NewsFramework.Services.Content.Feed
{
    [Serializable]
    public sealed class FeedPageRequest
    {
        public string feedId = "home";
        public string cursor;
        public int limit = 6;
        public string knownVersion;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public string GetParameter(string key)
        {
            if (string.IsNullOrEmpty(key) || parameters == null)
            {
                return string.Empty;
            }

            return parameters.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
