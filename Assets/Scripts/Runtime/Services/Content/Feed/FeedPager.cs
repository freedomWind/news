using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;
using UnityEngine;

namespace NewsFramework.Services.Content.Feed
{
    public sealed class FeedPager : IFeedPager
    {
        private readonly IFeedPageRemoteRepository remoteRepository;
        private readonly IFeedPageLocalRepository localRepository;
        private readonly IContentClock clock;
        private readonly List<BlockData> loadedBlocks = new List<BlockData>();
        private readonly HashSet<string> loadedBlockIds = new HashSet<string>();

        private FeedPagerConfig config = new FeedPagerConfig();
        private string nextCursor;
        private bool started;

        public bool IsActive { get; private set; }
        public bool IsLoading { get; private set; }
        public bool HasMore { get; private set; } = true;
        public string FeedVersion { get; private set; }
        public IReadOnlyList<BlockData> LoadedBlocks => loadedBlocks;

        public FeedPager(IFeedPageRemoteRepository remoteRepository)
            : this(remoteRepository, null, null)
        {
        }

        public FeedPager(
            IFeedPageRemoteRepository remoteRepository,
            IFeedPageLocalRepository localRepository,
            IContentClock clock)
        {
            this.remoteRepository = remoteRepository;
            this.localRepository = localRepository;
            this.clock = clock ?? new SystemContentClock();
        }

        public void Start(FeedPagerConfig pagerConfig, Action<FeedPageResult> onComplete)
        {
            config = pagerConfig ?? new FeedPagerConfig();
            IsActive = config.activeOnStart;
            started = true;
            nextCursor = string.Empty;
            HasMore = true;
            FeedVersion = string.Empty;
            loadedBlocks.Clear();
            loadedBlockIds.Clear();

            LoadNext(onComplete);
        }

        public void SetActive(bool active)
        {
            IsActive = active;
        }

        public void LoadNext(Action<FeedPageResult> onComplete)
        {
            if (!started)
            {
                Start(config, onComplete);
                return;
            }

            if (!IsActive || IsLoading || !HasMore)
            {
                return;
            }

            IsLoading = true;
            var request = new FeedPageRequest
            {
                feedId = config.feedId,
                cursor = nextCursor,
                limit = Mathf.Max(1, config.pageSize),
                knownVersion = FeedVersion
            };

            if (localRepository == null)
            {
                FetchRemote(request, onComplete);
                return;
            }

            localRepository.LoadPage(request, cachedResult => HandleLocalPageLoaded(request, cachedResult, onComplete));
        }

        public void EnsureAhead(float remainingContentPixels, float viewportHeight, Action<FeedPageResult> onComplete)
        {
            if (!IsActive || IsLoading || !HasMore)
            {
                return;
            }

            var safeViewportHeight = Mathf.Max(1f, viewportHeight);
            var threshold = safeViewportHeight * Mathf.Max(0.5f, config.prefetchScreens);
            if (remainingContentPixels <= threshold)
            {
                LoadNext(onComplete);
            }
        }

        private void HandleLocalPageLoaded(
            FeedPageRequest request,
            FeedPageResult cachedResult,
            Action<FeedPageResult> onComplete)
        {
            if (cachedResult == null || !cachedResult.success)
            {
                FetchRemote(request, onComplete);
                return;
            }

            var shouldFetchRemote = ShouldFetchRemoteAfterCache(request, cachedResult);
            HandlePageLoaded(cachedResult, onComplete, completeLoading: !shouldFetchRemote);

            if (shouldFetchRemote)
            {
                FetchRemote(request, onComplete);
            }
        }

        private bool ShouldFetchRemoteAfterCache(FeedPageRequest request, FeedPageResult cachedResult)
        {
            if (request == null || string.IsNullOrEmpty(request.cursor))
            {
                return true;
            }

            return cachedResult == null || cachedResult.IsExpired(clock.UnixSeconds);
        }

        private void FetchRemote(FeedPageRequest request, Action<FeedPageResult> onComplete)
        {
            if (remoteRepository == null)
            {
                IsLoading = false;
                onComplete?.Invoke(FeedPageResult.Failure("Feed remote repository is missing."));
                return;
            }

            remoteRepository.FetchPage(request, result => HandleRemotePageLoaded(result, onComplete));
        }

        private void HandleRemotePageLoaded(FeedPageResult result, Action<FeedPageResult> onComplete)
        {
            if (result != null && result.success && localRepository != null)
            {
                if (ShouldClearLocalFeed(result))
                {
                    localRepository.ClearFeed(result.feedId, _ => localRepository.SavePage(result, __ => { }));
                }
                else
                {
                    localRepository.SavePage(result, _ => { });
                }
            }

            HandlePageLoaded(result, onComplete, completeLoading: true);
        }

        private bool ShouldClearLocalFeed(FeedPageResult result)
        {
            return result != null
                && string.IsNullOrEmpty(result.cursor)
                && !string.IsNullOrEmpty(FeedVersion)
                && !string.IsNullOrEmpty(result.feedVersion)
                && FeedVersion != result.feedVersion;
        }

        private void HandlePageLoaded(FeedPageResult result, Action<FeedPageResult> onComplete, bool completeLoading)
        {
            if (completeLoading)
            {
                IsLoading = false;
            }

            if (result == null || !result.success)
            {
                onComplete?.Invoke(result ?? FeedPageResult.Failure("Feed page load failed."));
                return;
            }

            var versionChanged = !string.IsNullOrEmpty(FeedVersion)
                && !string.IsNullOrEmpty(result.feedVersion)
                && FeedVersion != result.feedVersion;

            if (versionChanged || string.IsNullOrEmpty(FeedVersion))
            {
                if (versionChanged)
                {
                    loadedBlocks.Clear();
                    loadedBlockIds.Clear();
                }

                FeedVersion = result.feedVersion;
                result.reset = true;
            }

            var appended = new List<BlockData>();
            if (result.blocks != null)
            {
                for (var i = 0; i < result.blocks.Count; i++)
                {
                    var block = result.blocks[i];
                    if (block == null)
                    {
                        continue;
                    }

                    var blockId = string.IsNullOrEmpty(block.id) ? Guid.NewGuid().ToString("N") : block.id;
                    if (!loadedBlockIds.Add(blockId))
                    {
                        continue;
                    }

                    loadedBlocks.Add(block);
                    appended.Add(block);
                }
            }

            result.blocks = appended;
            nextCursor = result.nextCursor;
            HasMore = result.hasMore;
            onComplete?.Invoke(result);
        }
    }
}
