using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;

namespace NewsFramework.Services.Content.Feed
{
    public interface IFeedPager
    {
        bool IsActive { get; }
        bool IsLoading { get; }
        bool HasMore { get; }
        string FeedVersion { get; }
        IReadOnlyList<BlockData> LoadedBlocks { get; }

        void Start(FeedPagerConfig config, Action<FeedPageResult> onComplete);
        void SetActive(bool active);
        void LoadNext(Action<FeedPageResult> onComplete);
        void EnsureAhead(float remainingContentPixels, float viewportHeight, Action<FeedPageResult> onComplete);
    }
}
