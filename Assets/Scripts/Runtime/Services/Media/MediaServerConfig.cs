using System;

namespace NewsFramework.Services.Media
{
    [Serializable]
    public sealed class MediaServerConfig
    {
        public bool enabled = true;
        public string baseUrl = "http://localhost:5234";
        public bool resolveRelativeUrls = true;

        public string ResolveUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            if (!enabled || IsMockUrl(url) || IsAbsoluteUrl(url))
            {
                return url;
            }

            if (!resolveRelativeUrls || !url.StartsWith("/", StringComparison.Ordinal))
            {
                return url;
            }

            var normalizedBase = (baseUrl ?? string.Empty).TrimEnd('/');
            return string.IsNullOrEmpty(normalizedBase) ? url : normalizedBase + url;
        }

        public string ResolveCacheKeyUrl(string url)
        {
            var resolved = ResolveUrl(url);
            return string.IsNullOrEmpty(resolved) ? url : resolved;
        }

        private static bool IsMockUrl(string url)
        {
            return url.StartsWith("mock://", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAbsoluteUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
