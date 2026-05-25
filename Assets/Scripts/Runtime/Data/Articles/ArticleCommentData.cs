using System;

namespace NewsFramework.Data.Articles
{
    [Serializable]
    public sealed class ArticleCommentData
    {
        public string commentId;
        public string authorName;
        public string avatarText;
        public string time;
        public string text;
        public ArticleCommentReplyData authorReply;
    }
}
