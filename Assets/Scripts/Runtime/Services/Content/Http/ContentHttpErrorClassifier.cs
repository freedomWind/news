using System;

namespace NewsFramework.Services.Content.Http
{
    public static class ContentHttpErrorClassifier
    {
        public static ContentHttpResponse Normalize(ContentHttpResponse response)
        {
            var normalized = response ?? new ContentHttpResponse
            {
                success = false,
                error = "HTTP response is empty."
            };

            if (normalized.success || normalized.errorKind == ContentHttpErrorKind.None)
            {
                normalized.errorKind = Classify(normalized);
            }

            normalized.retryable = IsRetryable(normalized.errorKind);
            if (!normalized.success && string.IsNullOrEmpty(normalized.error))
            {
                normalized.error = BuildDefaultError(normalized);
            }

            return normalized;
        }

        public static ContentHttpErrorKind Classify(ContentHttpResponse response)
        {
            if (response == null)
            {
                return ContentHttpErrorKind.EmptyResponse;
            }

            if (response.success)
            {
                return ContentHttpErrorKind.None;
            }

            if (response.statusCode == 0)
            {
                return LooksLikeTimeout(response.error)
                    ? ContentHttpErrorKind.Timeout
                    : ContentHttpErrorKind.Network;
            }

            if (response.statusCode == 401 || response.statusCode == 403)
            {
                return ContentHttpErrorKind.Unauthorized;
            }

            if (response.statusCode == 404)
            {
                return ContentHttpErrorKind.NotFound;
            }

            if (response.statusCode == 408)
            {
                return ContentHttpErrorKind.Timeout;
            }

            if (response.statusCode == 429)
            {
                return ContentHttpErrorKind.RateLimited;
            }

            if (response.statusCode >= 500)
            {
                return ContentHttpErrorKind.Server;
            }

            if (response.statusCode >= 400)
            {
                return ContentHttpErrorKind.HttpStatus;
            }

            return ContentHttpErrorKind.Network;
        }

        public static bool IsRetryable(ContentHttpErrorKind kind)
        {
            return kind == ContentHttpErrorKind.Network
                || kind == ContentHttpErrorKind.Timeout
                || kind == ContentHttpErrorKind.RateLimited
                || kind == ContentHttpErrorKind.Server;
        }

        public static string BuildFailureMessage(string prefix, ContentHttpResponse response)
        {
            var normalized = Normalize(response);
            var message = string.IsNullOrEmpty(prefix) ? "HTTP request failed" : prefix;
            message += " (" + normalized.errorKind + ")";

            if (normalized.statusCode > 0)
            {
                message += ", status=" + normalized.statusCode;
            }

            if (normalized.attemptCount > 1)
            {
                message += ", attempts=" + normalized.attemptCount;
            }

            if (!string.IsNullOrEmpty(normalized.error))
            {
                message += ": " + normalized.error;
            }

            return message;
        }

        private static bool LooksLikeTimeout(string error)
        {
            return !string.IsNullOrEmpty(error)
                && error.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string BuildDefaultError(ContentHttpResponse response)
        {
            if (response == null)
            {
                return "HTTP response is empty.";
            }

            return response.statusCode > 0
                ? "HTTP request failed with status " + response.statusCode
                : "HTTP request failed.";
        }
    }
}
