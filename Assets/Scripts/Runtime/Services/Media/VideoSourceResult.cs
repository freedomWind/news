using System;

namespace NewsFramework.Services.Media
{
    [Serializable]
    public sealed class VideoSourceResult
    {
        public bool success;
        public string error;
        public string mediaId;
        public string posterUrl;
        public string streamUrl;
        public string mimeType;
        public float durationSeconds;

        public static VideoSourceResult Success(MediaAssetRequest request)
        {
            return new VideoSourceResult
            {
                success = true,
                mediaId = request != null ? request.mediaId : string.Empty,
                posterUrl = request != null ? request.posterUrl : string.Empty,
                streamUrl = request != null ? request.streamUrl : string.Empty,
                mimeType = request != null ? request.mimeType : string.Empty,
                durationSeconds = request != null ? request.durationSeconds : 0f
            };
        }

        public static VideoSourceResult Failure(string error)
        {
            return new VideoSourceResult
            {
                success = false,
                error = error
            };
        }
    }
}
