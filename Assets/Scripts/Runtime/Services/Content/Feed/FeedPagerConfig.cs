using System;

namespace NewsFramework.Services.Content.Feed
{
    [Serializable]
    public sealed class FeedPagerConfig
    {
        public string feedId = "home";
        public int pageSize = 6;
        public float prefetchScreens = 2.5f;
        public bool activeOnStart = true;
    }
}
