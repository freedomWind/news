using System;
using UnityEngine;

namespace NewsFramework.Services.Media
{
    [Serializable]
    public sealed class MediaAssetResult
    {
        public bool success;
        public string error;
        public string cacheKey;
        public Texture2D texture;
        public bool fromCache;

        public static MediaAssetResult Success(Texture2D texture, string cacheKey, bool fromCache)
        {
            return new MediaAssetResult
            {
                success = true,
                texture = texture,
                cacheKey = cacheKey,
                fromCache = fromCache
            };
        }

        public static MediaAssetResult Failure(string error, string cacheKey = "")
        {
            return new MediaAssetResult
            {
                success = false,
                error = error,
                cacheKey = cacheKey
            };
        }
    }
}
