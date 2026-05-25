using System;
using System.Collections.Generic;

namespace NewsFramework.Services.Article.Dto
{
    [Serializable]
    public sealed class ArticleEngagementEnvelopeDto
    {
        public bool? success;
        public int code;
        public string message;
        public long serverTime;
        public ArticleEngagementSummaryDto data;

        public bool IsSuccess => success.HasValue ? success.Value : code == 0;
    }

    [Serializable]
    public sealed class ArticleEngagementSummaryDto
    {
        public string articleId;
        public bool bookmarked;
        public bool flowered;
        public bool canComment = true;
        public int flowerCount;
        public int commentCount;
        public List<ArticleCommentDto> previewComments = new List<ArticleCommentDto>();
    }

    [Serializable]
    public sealed class ArticleCommentDto
    {
        public string commentId;
        public string authorName;
        public string avatarText;
        public string time;
        public string text;
        public ArticleCommentReplyDto authorReply;
    }

    [Serializable]
    public sealed class ArticleCommentReplyDto
    {
        public string authorName;
        public string text;
    }
}
