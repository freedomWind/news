using System;
using NewsFramework.Data.Mock;

namespace NewsFramework.Services.Content.Feed
{
    public sealed class MockFeedPageRemoteRepository : IFeedPageRemoteRepository
    {
        public void FetchPage(FeedPageRequest request, Action<FeedPageResult> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.feedId))
            {
                onComplete?.Invoke(FeedPageResult.Failure("Feed request is empty."));
                return;
            }

            var offset = ParseCursor(request.cursor);
            var limit = request.limit <= 0 ? 6 : request.limit;
            var page = HomeMockData.CreatePaged(offset, limit, out var totalBlocks);
            var nextOffset = offset + (page.blocks != null ? page.blocks.Count : 0);
            var hasMore = nextOffset < totalBlocks;
            var version = ResolveVersion(request);

            onComplete?.Invoke(FeedPageResult.Success(
                request.feedId,
                page.title,
                version,
                request.cursor,
                hasMore ? nextOffset.ToString() : string.Empty,
                hasMore,
                totalBlocks,
                page.blocks));
        }

        private static int ParseCursor(string cursor)
        {
            return int.TryParse(cursor, out var offset) && offset > 0 ? offset : 0;
        }

        private static string ResolveVersion(FeedPageRequest request)
        {
            var serverVersion = request.GetParameter("serverVersion");
            return string.IsNullOrEmpty(serverVersion) ? request.feedId + "_v1" : serverVersion;
        }
    }
}
