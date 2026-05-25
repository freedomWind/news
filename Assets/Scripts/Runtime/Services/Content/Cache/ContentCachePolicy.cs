using System;

namespace NewsFramework.Services.Content.Cache
{
    [Serializable]
    public sealed class ContentCachePolicy
    {
        public long pageTtlSeconds = 7200;
        public long articleTtlSeconds = 86400;
        public long replayTtlSeconds = 604800;

        public long ResolveTtlSeconds(string pageType)
        {
            switch (pageType)
            {
                case ContentPageTypes.Article:
                    return articleTtlSeconds;
                case ContentPageTypes.Replay:
                    return replayTtlSeconds;
                default:
                    return pageTtlSeconds;
            }
        }
    }
}
