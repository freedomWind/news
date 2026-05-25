using System;
using UnityEngine;
using UnityEngine.Networking;

namespace NewsFramework.Services.Media
{
    public sealed class UnityMediaAssetService : IMediaAssetService
    {
        private readonly IMediaAssetCache cache;
        private readonly MediaServerConfig serverConfig;

        public UnityMediaAssetService(IMediaAssetCache cache)
            : this(cache, null)
        {
        }

        public UnityMediaAssetService(IMediaAssetCache cache, MediaServerConfig mediaServerConfig)
        {
            this.cache = cache ?? new MemoryMediaAssetCache();
            serverConfig = mediaServerConfig ?? new MediaServerConfig();
        }

        public void LoadTexture(MediaAssetRequest request, Action<MediaAssetResult> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.url))
            {
                onComplete?.Invoke(MediaAssetResult.Failure("Image url is empty."));
                return;
            }

            var resolvedUrl = serverConfig.ResolveUrl(request.url);
            var cacheKey = request.ResolveCacheKey(serverConfig);
            if (cache.TryGetTexture(cacheKey, out var cachedTexture))
            {
                onComplete?.Invoke(MediaAssetResult.Success(cachedTexture, cacheKey, fromCache: true));
                return;
            }

            if (resolvedUrl.StartsWith("mock://", StringComparison.OrdinalIgnoreCase))
            {
                var mockTexture = CreateMockTexture(resolvedUrl);
                cache.SetTexture(cacheKey, mockTexture);
                onComplete?.Invoke(MediaAssetResult.Success(mockTexture, cacheKey, fromCache: false));
                return;
            }

            var unityRequest = UnityWebRequestTexture.GetTexture(resolvedUrl);
            unityRequest.timeout = 12;
            var operation = unityRequest.SendWebRequest();
            operation.completed += _ =>
            {
                if (unityRequest.result != UnityWebRequest.Result.Success)
                {
                    var error = unityRequest.error;
                    unityRequest.Dispose();
                    onComplete?.Invoke(MediaAssetResult.Failure(error, cacheKey));
                    return;
                }

                var texture = DownloadHandlerTexture.GetContent(unityRequest);
                unityRequest.Dispose();

                if (texture == null)
                {
                    onComplete?.Invoke(MediaAssetResult.Failure("Image response is empty.", cacheKey));
                    return;
                }

                cache.SetTexture(cacheKey, texture);
                onComplete?.Invoke(MediaAssetResult.Success(texture, cacheKey, fromCache: false));
            };
        }

        public void ResolveVideoSource(MediaAssetRequest request, Action<VideoSourceResult> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.streamUrl))
            {
                onComplete?.Invoke(VideoSourceResult.Failure("Video stream url is empty."));
                return;
            }

            request.url = serverConfig.ResolveUrl(request.url);
            request.posterUrl = serverConfig.ResolveUrl(request.posterUrl);
            request.streamUrl = serverConfig.ResolveUrl(request.streamUrl);
            onComplete?.Invoke(VideoSourceResult.Success(request));
        }

        private static Texture2D CreateMockTexture(string key)
        {
            var texture = new Texture2D(64, 40, TextureFormat.RGBA32, false)
            {
                name = string.IsNullOrEmpty(key) ? "MockMediaTexture" : key
            };

            var baseColor = new Color(0.78f, 0.82f, 0.86f, 1f);
            var accentColor = new Color(0.48f, 0.56f, 0.66f, 1f);

            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var band = x > y * 1.2f && x < y * 1.2f + 12f;
                    texture.SetPixel(x, y, band ? accentColor : baseColor);
                }
            }

            texture.Apply(false, false);
            return texture;
        }
    }
}
