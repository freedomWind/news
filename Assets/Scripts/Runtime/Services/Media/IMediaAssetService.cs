using System;

namespace NewsFramework.Services.Media
{
    public interface IMediaAssetService
    {
        void LoadTexture(MediaAssetRequest request, Action<MediaAssetResult> onComplete);
        void ResolveVideoSource(MediaAssetRequest request, Action<VideoSourceResult> onComplete);
    }
}
