namespace NewsFramework.Content.Api.Contracts;

public sealed class ArticleEngagementSummaryData
{
    public string ArticleId { get; init; } = string.Empty;
    public bool Bookmarked { get; init; }
    public bool Flowered { get; init; }
    public bool CanComment { get; init; } = true;
    public int FlowerCount { get; init; }
    public int CommentCount { get; init; }
    public IReadOnlyList<CommentPreviewData> PreviewComments { get; init; } = Array.Empty<CommentPreviewData>();
}
