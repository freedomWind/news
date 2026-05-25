using System;
using NewsFramework.Data.Articles;

namespace NewsFramework.Services.Article
{
    public interface IArticleEngagementService
    {
        void LoadSummary(string articleId, Action<ArticleEngagementSummaryData> onComplete);
    }
}
