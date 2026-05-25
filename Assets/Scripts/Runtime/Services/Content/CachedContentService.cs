using System;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Content.Repositories;

namespace NewsFramework.Services.Content
{
    public sealed class CachedContentService : IContentService
    {
        private readonly IContentStore memoryStore;
        private readonly IContentLocalRepository localRepository;
        private readonly IContentRemoteRepository remoteRepository;
        private readonly IContentClock clock;

        public event Action<ContentResult> PageUpdated;

        public CachedContentService(
            IContentStore memoryStore,
            IContentLocalRepository localRepository,
            IContentRemoteRepository remoteRepository,
            IContentClock clock)
        {
            this.memoryStore = memoryStore;
            this.localRepository = localRepository;
            this.remoteRepository = remoteRepository;
            this.clock = clock ?? new SystemContentClock();
        }

        public void LoadPage(ContentRequest request, Action<ContentResult> onComplete)
        {
            if (!ValidateRequest(request, onComplete))
            {
                return;
            }

            var mode = ResolveRefreshMode(request);
            if (mode == ContentRefreshMode.ForceRemote)
            {
                RefreshPage(request, onComplete);
                return;
            }

            if (memoryStore != null && memoryStore.TryGetEntry(request.pageId, out var memoryEntry) && IsUsable(memoryEntry))
            {
                ReturnCached(memoryEntry, onComplete);
                CheckRemoteIfNeeded(request, memoryEntry, mode);
                return;
            }

            if (localRepository == null)
            {
                FetchRemote(request, null, onComplete, notifyOnlyWhenChanged: false);
                return;
            }

            localRepository.Load(request, localEntry =>
            {
                if (IsUsable(localEntry))
                {
                    memoryStore?.SetEntry(localEntry);
                    ReturnCached(localEntry, onComplete);
                    CheckRemoteIfNeeded(request, localEntry, mode);
                    return;
                }

                FetchRemote(request, null, onComplete, notifyOnlyWhenChanged: false);
            });
        }

        public void RefreshPage(ContentRequest request, Action<ContentResult> onComplete)
        {
            if (!ValidateRequest(request, onComplete))
            {
                return;
            }

            TryGetCachedEntry(request.pageId, out var cachedEntry);
            FetchRemote(request, cachedEntry, onComplete, notifyOnlyWhenChanged: false);
        }

        private void CheckRemoteIfNeeded(ContentRequest request, ContentCacheEntry cachedEntry, ContentRefreshMode mode)
        {
            if (!ShouldCheckRemote(request, cachedEntry, mode))
            {
                return;
            }

            FetchRemote(request, cachedEntry, DispatchUpdate, notifyOnlyWhenChanged: true);
        }

        private void FetchRemote(
            ContentRequest request,
            ContentCacheEntry cachedEntry,
            Action<ContentResult> onComplete,
            bool notifyOnlyWhenChanged)
        {
            if (remoteRepository == null)
            {
                onComplete?.Invoke(ContentResult.Failure("Remote repository is missing."));
                return;
            }

            remoteRepository.Fetch(request, remoteEntry =>
            {
                if (!IsUsable(remoteEntry))
                {
                    onComplete?.Invoke(ContentResult.Failure("Remote content not found: " + request.pageId));
                    return;
                }

                var changed = HasChanged(cachedEntry, remoteEntry);
                SaveEntry(remoteEntry);

                if (notifyOnlyWhenChanged && !changed)
                {
                    onComplete?.Invoke(ContentResult.Success(remoteEntry.page, remoteEntry.metadata, fromCache: false, changed: false));
                    return;
                }

                onComplete?.Invoke(ContentResult.Success(remoteEntry.page, remoteEntry.metadata, fromCache: false, changed: changed));
            });
        }

        private bool ShouldCheckRemote(ContentRequest request, ContentCacheEntry cachedEntry, ContentRefreshMode mode)
        {
            if (mode == ContentRefreshMode.AlwaysCheckRemote)
            {
                return true;
            }

            if (mode == ContentRefreshMode.CacheFirst)
            {
                return IsExpired(cachedEntry) || IsKnownVersionMismatch(request, cachedEntry);
            }

            return IsExpired(cachedEntry) || IsKnownVersionMismatch(request, cachedEntry);
        }

        private ContentRefreshMode ResolveRefreshMode(ContentRequest request)
        {
            if (request.refreshMode != ContentRefreshMode.Default)
            {
                return request.refreshMode;
            }

            return ContentRefreshMode.CacheFirst;
        }

        private bool IsExpired(ContentCacheEntry entry)
        {
            return entry == null || entry.metadata == null || entry.metadata.IsExpired(clock.UnixSeconds);
        }

        private static bool IsKnownVersionMismatch(ContentRequest request, ContentCacheEntry entry)
        {
            if (request == null || string.IsNullOrEmpty(request.knownVersion))
            {
                return false;
            }

            return entry == null
                || entry.metadata == null
                || string.IsNullOrEmpty(entry.metadata.version)
                || entry.metadata.version != request.knownVersion;
        }

        private static bool HasChanged(ContentCacheEntry cachedEntry, ContentCacheEntry remoteEntry)
        {
            if (cachedEntry == null || cachedEntry.metadata == null)
            {
                return true;
            }

            if (remoteEntry == null || remoteEntry.metadata == null)
            {
                return false;
            }

            if (cachedEntry.metadata.HasVersion() && remoteEntry.metadata.HasVersion())
            {
                return cachedEntry.metadata.version != remoteEntry.metadata.version;
            }

            return true;
        }

        private void SaveEntry(ContentCacheEntry entry)
        {
            memoryStore?.SetEntry(entry);
            localRepository?.Save(entry, _ => { });
        }

        private bool TryGetCachedEntry(string pageId, out ContentCacheEntry entry)
        {
            if (memoryStore != null && memoryStore.TryGetEntry(pageId, out entry) && IsUsable(entry))
            {
                return true;
            }

            entry = null;
            return false;
        }

        private static void ReturnCached(ContentCacheEntry entry, Action<ContentResult> onComplete)
        {
            onComplete?.Invoke(ContentResult.Success(entry.page, entry.metadata, fromCache: true, changed: false));
        }

        private static bool IsUsable(ContentCacheEntry entry)
        {
            return entry != null && entry.HasPage;
        }

        private static bool ValidateRequest(ContentRequest request, Action<ContentResult> onComplete)
        {
            if (request != null && !string.IsNullOrEmpty(request.pageId))
            {
                return true;
            }

            onComplete?.Invoke(ContentResult.Failure("Content request is empty."));
            return false;
        }

        private void DispatchUpdate(ContentResult result)
        {
            if (result != null && result.success && result.changed)
            {
                PageUpdated?.Invoke(result);
            }
        }
    }
}
