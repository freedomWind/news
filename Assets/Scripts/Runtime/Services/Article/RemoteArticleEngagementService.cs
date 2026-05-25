using System;
using NewsFramework.Data.Articles;
using NewsFramework.Services.Article.Repositories;

namespace NewsFramework.Services.Article
{
    public sealed class RemoteArticleEngagementService : IArticleEngagementService
    {
        private readonly IArticleEngagementRemoteRepository remoteRepository;

        public RemoteArticleEngagementService(IArticleEngagementRemoteRepository remoteRepository)
        {
            this.remoteRepository = remoteRepository;
        }

        public void LoadSummary(string articleId, Action<ArticleEngagementSummaryData> onComplete)
        {
            if (remoteRepository == null)
            {
                onComplete?.Invoke(null);
                return;
            }

            remoteRepository.FetchSummary(articleId, onComplete);
        }
    }
}
