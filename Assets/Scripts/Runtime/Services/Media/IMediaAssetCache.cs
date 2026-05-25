using UnityEngine;

namespace NewsFramework.Services.Media
{
    public interface IMediaAssetCache
    {
        bool TryGetTexture(string cacheKey, out Texture2D texture);
        void SetTexture(string cacheKey, Texture2D texture);
    }
}
