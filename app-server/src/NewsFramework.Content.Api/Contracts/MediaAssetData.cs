namespace NewsFramework.Content.Api.Contracts;

public sealed class MediaAssetData
{
    public string MediaId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public float AspectRatio { get; init; }
}
