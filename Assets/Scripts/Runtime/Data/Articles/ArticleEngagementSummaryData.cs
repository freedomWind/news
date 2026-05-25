using System;
using System.Collections.Generic;

namespace NewsFramework.Data.Articles
{
    [Serializable]
    public sealed class ArticleEngagementSummaryData
    {
        public string articleId;
        public bool bookmarked;
        public bool flowered;
        public bool canComment = true;
        public int flowerCount;
        public int commentCount;
        public List<ArticleCommentData> previewComments = new List<ArticleCommentData>();
    }
}
