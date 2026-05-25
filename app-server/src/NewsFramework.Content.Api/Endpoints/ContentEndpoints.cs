using NewsFramework.Content.Api.Contracts;
using NewsFramework.Content.Api.Repositories;

namespace NewsFramework.Content.Api.Endpoints;

public static class ContentEndpoints
{
    public static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/feed/home", GetHomeFeedAsync);
        routes.MapGet("/api/articles/{articleId}", GetArticleAsync);
        routes.MapGet("/api/articles/{articleId}/engagement-summary", GetArticleEngagementSummaryAsync);

        return routes;
    }

    private static async Task<IResult> GetHomeFeedAsync(
        string? cursor,
        int? limit,
        string? knownVersion,
        IContentRepository repository,
        CancellationToken cancellationToken)
    {
        var feed = await repository.GetHomeFeedAsync(
            cursor ?? string.Empty,
            limit ?? 10,
            knownVersion ?? string.Empty,
            cancellationToken);

        return Results.Ok(ApiResponse<FeedPageResult>.Ok(feed));
    }

    private static async Task<IResult> GetArticleAsync(
        string articleId,
        string? knownVersion,
        IContentRepository repository,
        CancellationToken cancellationToken)
    {
        var article = await repository.GetArticleAsync(articleId, knownVersion ?? string.Empty, cancellationToken);
        return article == null
            ? Results.NotFound(ApiResponse<PageData>.Fail("article not found"))
            : Results.Ok(ApiResponse<PageData>.Ok(article));
    }

    private static async Task<IResult> GetArticleEngagementSummaryAsync(
        string articleId,
        IContentRepository repository,
        CancellationToken cancellationToken)
    {
        var summary = await repository.GetArticleEngagementSummaryAsync(articleId, cancellationToken);
        return summary == null
            ? Results.NotFound(new ErrorResponse { Error = "article engagement summary not found" })
            : Results.Ok(summary);
    }
}
