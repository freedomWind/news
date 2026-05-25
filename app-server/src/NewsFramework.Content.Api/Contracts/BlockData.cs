namespace NewsFramework.Content.Api.Contracts;

public sealed class BlockData
{
    public string BlockId { get; init; } = string.Empty;
    public string BlockType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public string ArticleId { get; init; } = string.Empty;
    public string ActionUrl { get; init; } = string.Empty;
    public MediaAssetData? Media { get; init; }
    public IReadOnlyList<MediaAssetData> MediaItems { get; init; } = Array.Empty<MediaAssetData>();
    public Dictionary<string, object> Metadata { get; init; } = new();
}
