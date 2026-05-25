using NewsFramework.Content.Api.Contracts;

namespace NewsFramework.Content.Api.Repositories;

public interface IContentRepository
{
    Task<FeedPageResult> GetHomeFeedAsync(string cursor, int limit, string knownVersion, CancellationToken cancellationToken);
    Task<PageData?> GetArticleAsync(string articleId, string knownVersion, CancellationToken cancellationToken);
    Task<ArticleEngagementSummaryData?> GetArticleEngagementSummaryAsync(string articleId, CancellationToken cancellationToken);
}
