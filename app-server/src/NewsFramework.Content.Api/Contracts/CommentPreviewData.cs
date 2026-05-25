namespace NewsFramework.Content.Api.Contracts;

public sealed class CommentPreviewData
{
    public string CommentId { get; init; } = string.Empty;
    public string AuthorId { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public long CreatedAtUnixSeconds { get; init; }
}
