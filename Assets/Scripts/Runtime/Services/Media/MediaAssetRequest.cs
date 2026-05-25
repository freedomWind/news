using System;
using NewsFramework.Data.Blocks;

namespace NewsFramework.Services.Media
{
    [Serializable]
    public sealed class MediaAssetRequest
    {
        public string mediaId;
        public string mediaType = MediaAssetTypes.Image;
        public string url;
        public string posterUrl;
        public string streamUrl;
        public string version;
        public string hash;
        public string mimeType;
        public float aspectRatio;
        public float durationSeconds;

        public static MediaAssetRequest Image(BlockData block)
        {
            var media = block?.media;
            return new MediaAssetRequest
            {
                mediaId = media != null ? media.mediaId : string.Empty,
                mediaType = MediaAssetTypes.Image,
                url = !string.IsNullOrEmpty(media?.url) ? media.url : block?.url,
                version = media != null ? media.version : string.Empty,
                hash = media != null ? media.hash : string.Empty,
                mimeType = media != null ? media.mimeType : string.Empty,
                aspectRatio = media != null && media.aspectRatio > 0f ? media.aspectRatio : block?.aspectRatio ?? 1.6f
            };
        }

        public static MediaAssetRequest Video(BlockData block)
        {
            var media = block?.media;
            return new MediaAssetRequest
            {
                mediaId = media != null ? media.mediaId : string.Empty,
                mediaType = MediaAssetTypes.Video,
                url = !string.IsNullOrEmpty(media?.url) ? media.url : block?.url,
                posterUrl = !string.IsNullOrEmpty(media?.posterUrl) ? media.posterUrl : block?.posterUrl,
                streamUrl = !string.IsNullOrEmpty(media?.streamUrl) ? media.streamUrl : block?.streamUrl,
                version = media != null ? media.version : string.Empty,
                hash = media != null ? media.hash : string.Empty,
                mimeType = media != null ? media.mimeType : string.Empty,
                aspectRatio = media != null && media.aspectRatio > 0f ? media.aspectRatio : block?.aspectRatio ?? 1.777f,
                durationSeconds = media != null && media.durationSeconds > 0f ? media.durationSeconds : block?.durationSeconds ?? 0f
            };
        }

        public string ResolveCacheKey()
        {
            return ResolveCacheKey(null);
        }

        public string ResolveCacheKey(MediaServerConfig serverConfig)
        {
            if (!string.IsNullOrEmpty(mediaId))
            {
                return mediaType + ":" + mediaId + ":" + version;
            }

            if (!string.IsNullOrEmpty(hash))
            {
                return mediaType + ":hash:" + hash;
            }

            var cacheUrl = serverConfig != null ? serverConfig.ResolveCacheKeyUrl(url) : url;
            return mediaType + ":url:" + cacheUrl + ":" + version;
        }
    }
}
