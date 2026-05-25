using System;
using NewsFramework.Services.Content.Cache;
using NewsFramework.Services.Content.Dto;
using NewsFramework.Services.Content.Http;
using NewsFramework.Services.Content.Serialization;
using UnityEngine;

namespace NewsFramework.Services.Content.Repositories
{
    public sealed class HttpContentRemoteRepository : IContentRemoteRepository
    {
        private readonly ContentHttpConfig config;
        private readonly IContentHttpClient httpClient;
        private readonly IContentJsonSerializer jsonSerializer;
        private readonly ContentServerDtoMapper mapper;

        public HttpContentRemoteRepository(
            ContentHttpConfig config,
            IContentHttpClient httpClient,
            IContentJsonSerializer jsonSerializer,
            ContentCachePolicy cachePolicy,
            IContentClock clock)
        {
            this.config = config ?? new ContentHttpConfig();
            this.httpClient = httpClient;
            this.jsonSerializer = jsonSerializer ?? new NewtonsoftContentJsonSerializer();
            mapper = new ContentServerDtoMapper(cachePolicy, clock);
        }

        public void Fetch(ContentRequest request, Action<ContentCacheEntry> onComplete)
        {
            var url = config.BuildUrl(request);
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("HTTP content URL is empty.");
                onComplete?.Invoke(null);
                return;
            }

            if (httpClient == null)
            {
                Debug.LogWarning("Content HTTP client is missing.");
                onComplete?.Invoke(null);
                return;
            }

            var httpRequest = new ContentHttpRequest
            {
                method = ContentHttpMethods.Get,
                url = url,
                timeoutSeconds = config.timeoutSeconds
            };
            config.CopyHeadersTo(httpRequest);

            httpClient.Send(httpRequest, response => HandleResponse(request, response, onComplete));
        }

        private void HandleResponse(
            ContentRequest request,
            ContentHttpResponse response,
            Action<ContentCacheEntry> onComplete)
        {
            if (response == null || !response.success)
            {
                Debug.LogWarning(ContentHttpErrorClassifier.BuildFailureMessage("HTTP content request failed", response));
                onComplete?.Invoke(null);
                return;
            }

            ContentServerEnvelopeDto envelope;
            try
            {
                envelope = jsonSerializer.FromJson<ContentServerEnvelopeDto>(response.body);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("HTTP content JSON parse failed: " + exception.Message);
                onComplete?.Invoke(null);
                return;
            }

            if (!mapper.TryMap(envelope, request, out var entry, out var error))
            {
                Debug.LogWarning("HTTP content map failed: " + error);
                onComplete?.Invoke(null);
                return;
            }

            onComplete?.Invoke(entry);
        }
    }
}
