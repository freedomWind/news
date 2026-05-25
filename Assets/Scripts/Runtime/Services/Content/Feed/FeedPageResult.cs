using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;

namespace NewsFramework.Services.Content.Feed
{
    [Serializable]
    public sealed class FeedPageResult
    {
        public bool success;
        public string error;
        public string feedId;
        public string title;
        public string feedVersion;
        public string cursor;
        public string nextCursor;
        public bool hasMore;
        public int estimatedTotal;
        public bool reset;
        public bool fromCache;
        public long fetchedAtUnixSeconds;
        public long expiresAtUnixSeconds;
        public List<BlockData> blocks = new List<BlockData>();

        public static FeedPageResult Success(
            string feedId,
            string title,
            string feedVersion,
            string cursor,
            string nextCursor,
            bool hasMore,
            int estimatedTotal,
            List<BlockData> blocks,
            bool reset = false,
            bool fromCache = false,
            long fetchedAtUnixSeconds = 0,
            long expiresAtUnixSeconds = 0)
        {
            return new FeedPageResult
            {
                success = true,
                feedId = feedId,
                title = title,
                feedVersion = feedVersion,
                cursor = cursor,
                nextCursor = nextCursor,
                hasMore = hasMore,
                estimatedTotal = estimatedTotal,
                blocks = blocks ?? new List<BlockData>(),
                reset = reset,
                fromCache = fromCache,
                fetchedAtUnixSeconds = fetchedAtUnixSeconds,
                expiresAtUnixSeconds = expiresAtUnixSeconds
            };
        }

        public bool IsExpired(long nowUnixSeconds)
        {
            return expiresAtUnixSeconds > 0 && nowUnixSeconds >= expiresAtUnixSeconds;
        }

        public static FeedPageResult Failure(string error)
        {
            return new FeedPageResult
            {
                success = false,
                error = error
            };
        }
    }
}
