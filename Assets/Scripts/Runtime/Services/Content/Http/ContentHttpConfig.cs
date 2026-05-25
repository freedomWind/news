using System;
using System.Collections.Generic;

namespace NewsFramework.Services.Content.Http
{
    [Serializable]
    public sealed class ContentHttpConfig
    {
        public string baseUrl;
        public string feedPathTemplate = "/api/feed/{pageId}";
        public string articlePathTemplate = "/api/articles/{pageId}";
        public string articleEngagementPathTemplate = "/api/articles/{articleId}/engagement-summary";
        public string replayPathTemplate = "/api/content/replay/{pageId}";
        public string pagePathTemplate = "/api/content/page/{pageId}";
        public int timeoutSeconds = 10;
        public int maxRetryCount = 1;
        public List<ContentHttpHeader> headers = new List<ContentHttpHeader>();

        public string BuildUrl(ContentRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.pageId) || string.IsNullOrEmpty(baseUrl))
            {
                return string.Empty;
            }

            var template = ResolvePathTemplate(request);
            var escapedPageId = Uri.EscapeDataString(request.pageId);
            var path = ReplacePathId(template, "pageId", escapedPageId);
            path = ReplacePathId(path, "articleId", escapedPageId);
            var url = CombineUrl(baseUrl, path);

            url = AppendQuery(url, "contentKind", request.contentKind);
            url = AppendQuery(url, "pageType", request.pageType);
            url = AppendQuery(url, "knownVersion", request.knownVersion);

            if (request.parameters != null)
            {
                foreach (var pair in request.parameters)
                {
                    url = AppendQuery(url, pair.Key, pair.Value);
                }
            }

            return url;
        }

        public string BuildFeedPageUrl(
            string feedId,
            string cursor,
            int limit,
            string knownVersion,
            Dictionary<string, string> parameters = null)
        {
            if (string.IsNullOrEmpty(feedId) || string.IsNullOrEmpty(baseUrl))
            {
                return string.Empty;
            }

            var escapedFeedId = Uri.EscapeDataString(feedId);
            var path = ReplacePathId(feedPathTemplate, "pageId", escapedFeedId);
            path = ReplacePathId(path, "feedId", escapedFeedId);
            var url = CombineUrl(baseUrl, path);

            url = AppendQuery(url, "cursor", cursor ?? string.Empty);
            url = AppendQuery(url, "limit", limit > 0 ? limit.ToString() : string.Empty);
            url = AppendQuery(url, "knownVersion", knownVersion);

            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    url = AppendQuery(url, pair.Key, pair.Value);
                }
            }

            return url;
        }

        public string BuildArticleEngagementUrl(string articleId, Dictionary<string, string> parameters = null)
        {
            if (string.IsNullOrEmpty(articleId) || string.IsNullOrEmpty(baseUrl))
            {
                return string.Empty;
            }

            var escapedArticleId = Uri.EscapeDataString(articleId);
            var path = ReplacePathId(articleEngagementPathTemplate, "articleId", escapedArticleId);
            path = ReplacePathId(path, "pageId", escapedArticleId);
            var url = CombineUrl(baseUrl, path);

            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    url = AppendQuery(url, pair.Key, pair.Value);
                }
            }

            return url;
        }

        public void CopyHeadersTo(ContentHttpRequest request)
        {
            if (request == null || headers == null)
            {
                return;
            }

            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                if (header == null || string.IsNullOrEmpty(header.key))
                {
                    continue;
                }

                request.headers.Add(new ContentHttpHeader
                {
                    key = header.key,
                    value = header.value
                });
            }
        }

        private string ResolvePathTemplate(ContentRequest request)
        {
            switch (request.contentKind)
            {
                case ContentKinds.Article:
                    return articlePathTemplate;
                case ContentKinds.Replay:
                    return replayPathTemplate;
                default:
                    return pagePathTemplate;
            }
        }

        private static string CombineUrl(string root, string path)
        {
            return root.TrimEnd('/') + "/" + path.TrimStart('/');
        }

        private static string ReplacePathId(string path, string key, string escapedValue)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(key))
            {
                return path;
            }

            return path.Replace("{" + key + "}", escapedValue ?? string.Empty);
        }

        private static string AppendQuery(string url, string key, string value)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                return url;
            }

            var separator = url.Contains("?") ? "&" : "?";
            return url
                + separator
                + Uri.EscapeDataString(key)
                + "="
                + Uri.EscapeDataString(value);
        }
    }
}
