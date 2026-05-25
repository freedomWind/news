namespace NewsFramework.Media.Api.Contracts;

public sealed class MediaVariantDto
{
    public string VariantId { get; init; } = string.Empty;
    public string MediaId { get; init; } = string.Empty;
    public string Kind { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public float DurationSeconds { get; init; }
}
