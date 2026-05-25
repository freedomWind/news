using System;
using NewsFramework.Data.Articles;

namespace NewsFramework.Services.Article.Repositories
{
    public interface IArticleEngagementRemoteRepository
    {
        void FetchSummary(string articleId, Action<ArticleEngagementSummaryData> onComplete);
    }
}
