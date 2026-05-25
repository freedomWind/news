using System;
using System.Collections.Generic;

namespace NewsFramework.Services.Content
{
    [Serializable]
    public sealed class ContentRequest
    {
        public string pageId;
        public string pageType;
        public string contentKind = ContentKinds.Page;
        public string knownVersion;
        public ContentRefreshMode refreshMode = ContentRefreshMode.Default;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public static ContentRequest Page(string pageId, string pageType = "")
        {
            return new ContentRequest
            {
                pageId = pageId,
                pageType = string.IsNullOrEmpty(pageType) ? Cache.ContentPageTypes.Page : pageType,
                contentKind = ContentKinds.Page
            };
        }

        public static ContentRequest Article(string articleId, string knownVersion = "")
        {
            return new ContentRequest
            {
                pageId = articleId,
                pageType = Cache.ContentPageTypes.Article,
                contentKind = ContentKinds.Article,
                knownVersion = knownVersion,
                refreshMode = ContentRefreshMode.CacheFirst
            };
        }

        public static ContentRequest Replay(string replayId, string knownVersion = "")
        {
            return new ContentRequest
            {
                pageId = replayId,
                pageType = Cache.ContentPageTypes.Replay,
                contentKind = ContentKinds.Replay,
                knownVersion = knownVersion,
                refreshMode = ContentRefreshMode.CacheFirst
            };
        }

        public string GetParameter(string key)
        {
            if (string.IsNullOrEmpty(key) || parameters == null)
            {
                return string.Empty;
            }

            return parameters.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
