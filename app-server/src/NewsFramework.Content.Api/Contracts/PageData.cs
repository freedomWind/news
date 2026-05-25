namespace NewsFramework.Content.Api.Contracts;

public sealed class PageData
{
    public string PageId { get; init; } = string.Empty;
    public string PageType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public int ExpiresInSeconds { get; init; }
    public IReadOnlyList<BlockData> Blocks { get; init; } = Array.Empty<BlockData>();
}
