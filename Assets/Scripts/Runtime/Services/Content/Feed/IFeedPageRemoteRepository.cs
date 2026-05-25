using System;

namespace NewsFramework.Services.Content.Feed
{
    public interface IFeedPageRemoteRepository
    {
        void FetchPage(FeedPageRequest request, Action<FeedPageResult> onComplete);
    }
}
