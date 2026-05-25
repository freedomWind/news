namespace NewsFramework.Content.Api.Contracts;

public sealed class FeedPageResult
{
    public string FeedId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string FeedVersion { get; init; } = string.Empty;
    public string Cursor { get; init; } = string.Empty;
    public string NextCursor { get; init; } = string.Empty;
    public bool HasMore { get; init; }
    public int EstimatedTotal { get; init; }
    public int ExpiresInSeconds { get; init; }
    public IReadOnlyList<BlockData> Blocks { get; init; } = Array.Empty<BlockData>();
}
