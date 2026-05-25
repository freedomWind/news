using System;

namespace NewsFramework.Services.Content.Feed
{
    public interface IFeedPageLocalRepository
    {
        void LoadPage(FeedPageRequest request, Action<FeedPageResult> onComplete);
        void SavePage(FeedPageResult result, Action<bool> onComplete);
        void ClearFeed(string feedId, Action<bool> onComplete);
    }
}
