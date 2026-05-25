using System;

namespace NewsFramework.Services.Content.Cache
{
    [Serializable]
    public sealed class ContentCacheMetadata
    {
        public string pageId;
        public string pageType;
        public string version;
        public long fetchedAtUnixSeconds;
        public long expiresAtUnixSeconds;
        public string source;

        public bool IsExpired(long nowUnixSeconds)
        {
            return expiresAtUnixSeconds > 0 && nowUnixSeconds >= expiresAtUnixSeconds;
        }

        public bool HasVersion()
        {
            return !string.IsNullOrEmpty(version);
        }
    }
}
