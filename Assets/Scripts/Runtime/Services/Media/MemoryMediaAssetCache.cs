using System.Collections.Generic;
using UnityEngine;

namespace NewsFramework.Services.Media
{
    public sealed class MemoryMediaAssetCache : IMediaAssetCache
    {
        private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public bool TryGetTexture(string cacheKey, out Texture2D texture)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                texture = null;
                return false;
            }

            if (textures.TryGetValue(cacheKey, out texture) && texture != null)
            {
                return true;
            }

            textures.Remove(cacheKey);
            texture = null;
            return false;
        }

        public void SetTexture(string cacheKey, Texture2D texture)
        {
            if (string.IsNullOrEmpty(cacheKey) || texture == null)
            {
                return;
            }

            textures[cacheKey] = texture;
        }
    }
}
