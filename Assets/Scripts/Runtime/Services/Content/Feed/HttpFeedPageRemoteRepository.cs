using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Dto;
using NewsFramework.Services.Content.Http;
using NewsFramework.Services.Content.Serialization;
using UnityEngine;

namespace NewsFramework.Services.Content.Feed
{
    public sealed class HttpFeedPageRemoteRepository : IFeedPageRemoteRepository
    {
        private readonly ContentHttpConfig config;
        private readonly IContentHttpClient httpClient;
        private readonly IContentJsonSerializer jsonSerializer;

        public HttpFeedPageRemoteRepository(
            ContentHttpConfig config,
            IContentHttpClient httpClient,
            IContentJsonSerializer jsonSerializer)
        {
            this.config = config ?? new ContentHttpConfig();
            this.httpClient = httpClient;
            this.jsonSerializer = jsonSerializer ?? new NewtonsoftContentJsonSerializer();
        }

        public void FetchPage(FeedPageRequest request, Action<FeedPageResult> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.feedId))
            {
                onComplete?.Invoke(FeedPageResult.Failure("Feed request is empty."));
                return;
            }

            if (httpClient == null)
            {
                onComplete?.Invoke(FeedPageResult.Failure("Content HTTP client is missing."));
                return;
            }

            var url = BuildUrl(request);
            if (string.IsNullOrEmpty(url))
            {
                onComplete?.Invoke(FeedPageResult.Failure("Feed HTTP URL is empty."));
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

        private string BuildUrl(FeedPageRequest request)
        {
            return config.BuildFeedPageUrl(
                request.feedId,
                request.cursor,
                request.limit,
                request.knownVersion,
                request.parameters);
        }

        private void HandleResponse(
            FeedPageRequest request,
            ContentHttpResponse response,
            Action<FeedPageResult> onComplete)
        {
            if (response == null || !response.success)
            {
                var error = ContentHttpErrorClassifier.BuildFailureMessage("HTTP feed page request failed", response);
                Debug.LogWarning(error);
                onComplete?.Invoke(FeedPageResult.Failure(error));
                return;
            }

            ContentServerEnvelopeDto envelope;
            try
            {
                envelope = jsonSerializer.FromJson<ContentServerEnvelopeDto>(response.body);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("HTTP feed JSON parse failed: " + exception.Message);
                onComplete?.Invoke(FeedPageResult.Failure(exception.Message));
                return;
            }

            if (envelope == null || !envelope.IsSuccess || envelope.data == null)
            {
                var error = envelope != null && !string.IsNullOrEmpty(envelope.message)
                    ? envelope.message
                    : "HTTP feed response is invalid.";
                onComplete?.Invoke(FeedPageResult.Failure(error));
                return;
            }

            var data = envelope.data;
            var blocks = new List<BlockData>();
            if (data.blocks != null)
            {
                for (var i = 0; i < data.blocks.Count; i++)
                {
                    var block = ContentServerDtoMapper.MapBlock(data.blocks[i]);
                    if (block != null)
                    {
                        blocks.Add(block);
                    }
                }
            }

            var feedId = !string.IsNullOrEmpty(data.feedId)
                ? data.feedId
                : !string.IsNullOrEmpty(data.pageId) ? data.pageId : request.feedId;
            var feedVersion = !string.IsNullOrEmpty(data.feedVersion) ? data.feedVersion : data.version;
            var cursor = !string.IsNullOrEmpty(data.cursor) ? data.cursor : request.cursor;
            var fetchedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var expiresAt = data.expiresInSeconds > 0 ? fetchedAt + data.expiresInSeconds : 0;

            onComplete?.Invoke(FeedPageResult.Success(
                feedId,
                data.title,
                feedVersion,
                cursor,
                data.nextCursor,
                data.hasMore,
                data.estimatedTotal,
                blocks,
                fetchedAtUnixSeconds: fetchedAt,
                expiresAtUnixSeconds: expiresAt));
        }
    }
}
