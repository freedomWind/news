using System;

namespace NewsFramework.Services.Content.Http
{
    public sealed class RetryingContentHttpClient : IContentHttpClient
    {
        private readonly IContentHttpClient inner;
        private readonly int maxRetryCount;

        public RetryingContentHttpClient(IContentHttpClient inner, int maxRetryCount)
        {
            this.inner = inner;
            this.maxRetryCount = Math.Max(0, maxRetryCount);
        }

        public void Send(ContentHttpRequest request, Action<ContentHttpResponse> onComplete)
        {
            if (inner == null)
            {
                onComplete?.Invoke(ContentHttpErrorClassifier.Normalize(new ContentHttpResponse
                {
                    success = false,
                    errorKind = ContentHttpErrorKind.InvalidRequest,
                    error = "Inner HTTP client is missing."
                }));
                return;
            }

            SendAttempt(request, 1, onComplete);
        }

        private void SendAttempt(
            ContentHttpRequest request,
            int attempt,
            Action<ContentHttpResponse> onComplete)
        {
            inner.Send(request, response =>
            {
                var normalized = ContentHttpErrorClassifier.Normalize(response);
                normalized.attemptCount = attempt;

                if (ShouldRetry(request, normalized, attempt))
                {
                    SendAttempt(request, attempt + 1, onComplete);
                    return;
                }

                onComplete?.Invoke(normalized);
            });
        }

        private bool ShouldRetry(ContentHttpRequest request, ContentHttpResponse response, int attempt)
        {
            if (request == null || response == null || response.success || !response.retryable)
            {
                return false;
            }

            if (attempt > maxRetryCount)
            {
                return false;
            }

            return IsSafeRetryMethod(request.method);
        }

        private static bool IsSafeRetryMethod(string method)
        {
            return string.IsNullOrEmpty(method)
                || string.Equals(method, ContentHttpMethods.Get, StringComparison.OrdinalIgnoreCase)
                || string.Equals(method, ContentHttpMethods.Head, StringComparison.OrdinalIgnoreCase);
        }
    }
}
