using NewsFramework.Data.Articles;

namespace NewsFramework.Services.Article.Dto
{
    public static class ArticleEngagementServerDtoMapper
    {
        public static ArticleEngagementSummaryData MapSummary(
            ArticleEngagementSummaryDto dto,
            string fallbackArticleId)
        {
            if (dto == null)
            {
                return null;
            }

            var summary = new ArticleEngagementSummaryData
            {
                articleId = string.IsNullOrEmpty(dto.articleId) ? fallbackArticleId : dto.articleId,
                bookmarked = dto.bookmarked,
                flowered = dto.flowered,
                canComment = dto.canComment,
                flowerCount = dto.flowerCount,
                commentCount = dto.commentCount
            };

            if (dto.previewComments == null)
            {
                return summary;
            }

            for (var i = 0; i < dto.previewComments.Count; i++)
            {
                var comment = MapComment(dto.previewComments[i]);
                if (comment != null)
                {
                    summary.previewComments.Add(comment);
                }
            }

            return summary;
        }

        private static ArticleCommentData MapComment(ArticleCommentDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new ArticleCommentData
            {
                commentId = dto.commentId,
                authorName = dto.authorName,
                avatarText = dto.avatarText,
                time = dto.time,
                text = dto.text,
                authorReply = MapReply(dto.authorReply)
            };
        }

        private static ArticleCommentReplyData MapReply(ArticleCommentReplyDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new ArticleCommentReplyData
            {
                authorName = dto.authorName,
                text = dto.text
            };
        }
    }
}
