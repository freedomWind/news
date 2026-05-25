using System;
using NewsFramework.Data.Articles;

namespace NewsFramework.Services.Article
{
    public sealed class MockArticleEngagementService : IArticleEngagementService
    {
        public void LoadSummary(string articleId, Action<ArticleEngagementSummaryData> onComplete)
        {
            var summary = new ArticleEngagementSummaryData
            {
                articleId = articleId,
                bookmarked = articleId == "article_002",
                flowered = false,
                canComment = true,
                flowerCount = 128,
                commentCount = 8
            };

            summary.previewComments.Add(new ArticleCommentData
            {
                commentId = "comment_001",
                authorName = "老棋迷张伯",
                avatarText = "张",
                time = "2小时前",
                text = "柳老的棋确实有电脑风范，弃子果断，佩服佩服！"
            });

            summary.previewComments.Add(new ArticleCommentData
            {
                commentId = "comment_002",
                authorName = "象棋研究员",
                avatarText = "研",
                time = "5小时前",
                text = "这篇文章分析得透彻，特别是对第12回合的解读，受教了。",
                authorReply = new ArticleCommentReplyData
                {
                    authorName = "作者回复",
                    text = "感谢支持，多看名家复盘对提高大局观确实大有裨益。"
                }
            });

            summary.previewComments.Add(new ArticleCommentData
            {
                commentId = "comment_003",
                authorName = "弈林散人",
                avatarText = "弈",
                time = "昨天",
                text = "希望多出一些这类中局战术的讲解。"
            });

            onComplete?.Invoke(summary);
        }
    }
}
