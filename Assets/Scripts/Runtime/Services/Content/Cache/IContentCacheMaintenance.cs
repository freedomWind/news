using System;

namespace NewsFramework.Services.Content.Cache
{
    public interface IContentCacheMaintenance
    {
        void GetStats(Action<ContentCacheStats> onComplete);
        void Cleanup(ContentCacheMaintenancePolicy policy, Action<ContentCacheMaintenanceResult> onComplete);
    }
}
