using System;
using NewsFramework.Data.Articles;
using NewsFramework.Services.Article.Dto;
using NewsFramework.Services.Content.Http;
using NewsFramework.Services.Content.Serialization;
using UnityEngine;

namespace NewsFramework.Services.Article.Repositories
{
    public sealed class HttpArticleEngagementRemoteRepository : IArticleEngagementRemoteRepository
    {
        private readonly ContentHttpConfig config;
        private readonly IContentHttpClient httpClient;
        private readonly IContentJsonSerializer jsonSerializer;

        public HttpArticleEngagementRemoteRepository(
            ContentHttpConfig config,
            IContentHttpClient httpClient,
            IContentJsonSerializer jsonSerializer)
        {
            this.config = config ?? new ContentHttpConfig();
            this.httpClient = httpClient;
            this.jsonSerializer = jsonSerializer ?? new NewtonsoftContentJsonSerializer();
        }

        public void FetchSummary(string articleId, Action<ArticleEngagementSummaryData> onComplete)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                Debug.LogWarning("Article engagement request article id is empty.");
                onComplete?.Invoke(null);
                return;
            }

            if (httpClient == null)
            {
                Debug.LogWarning("Article engagement HTTP client is missing.");
                onComplete?.Invoke(null);
                return;
            }

            var url = config.BuildArticleEngagementUrl(articleId);
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("Article engagement HTTP URL is empty.");
                onComplete?.Invoke(null);
                return;
            }

            var request = new ContentHttpRequest
            {
                method = ContentHttpMethods.Get,
                url = url,
                timeoutSeconds = config.timeoutSeconds
            };
            config.CopyHeadersTo(request);

            httpClient.Send(request, response => HandleResponse(articleId, response, onComplete));
        }

        private void HandleResponse(
            string articleId,
            ContentHttpResponse response,
            Action<ArticleEngagementSummaryData> onComplete)
        {
            if (response == null || !response.success)
            {
                Debug.LogWarning(ContentHttpErrorClassifier.BuildFailureMessage("HTTP article engagement request failed", response));
                onComplete?.Invoke(null);
                return;
            }

            if (!TryParseSummary(articleId, response.body, out var summary, out var error))
            {
                Debug.LogWarning("HTTP article engagement parse failed: " + error);
                onComplete?.Invoke(null);
                return;
            }

            onComplete?.Invoke(summary);
        }

        private bool TryParseSummary(
            string articleId,
            string body,
            out ArticleEngagementSummaryData summary,
            out string error)
        {
            summary = null;
            error = string.Empty;

            try
            {
                var envelope = jsonSerializer.FromJson<ArticleEngagementEnvelopeDto>(body);
                if (envelope != null && envelope.data != null)
                {
                    if (!envelope.IsSuccess)
                    {
                        error = string.IsNullOrEmpty(envelope.message)
                            ? "Article engagement response code is " + envelope.code
                            : envelope.message;
                        return false;
                    }

                    summary = ArticleEngagementServerDtoMapper.MapSummary(envelope.data, articleId);
                    return summary != null;
                }

                if (envelope != null && envelope.success.HasValue && !envelope.IsSuccess)
                {
                    error = string.IsNullOrEmpty(envelope.message)
                        ? "Article engagement response is not successful."
                        : envelope.message;
                    return false;
                }

                var direct = jsonSerializer.FromJson<ArticleEngagementSummaryDto>(body);
                if (direct == null || string.IsNullOrEmpty(direct.articleId))
                {
                    error = "Article engagement response is invalid.";
                    return false;
                }

                summary = ArticleEngagementServerDtoMapper.MapSummary(direct, articleId);
                if (summary == null || string.IsNullOrEmpty(summary.articleId))
                {
                    error = "Article engagement response is invalid.";
                    summary = null;
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }
        }
    }
}
