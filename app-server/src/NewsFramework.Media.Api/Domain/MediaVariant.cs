namespace NewsFramework.Media.Api.Domain;

public sealed class MediaVariant
{
    public string VariantId { get; init; } = string.Empty;
    public string MediaId { get; init; } = string.Empty;
    public MediaVariantKind Kind { get; init; }
    public MediaAssetStatus Status { get; set; }
    public string ObjectKey { get; init; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public float DurationSeconds { get; set; }
    public long CreatedAtUnixSeconds { get; init; }
}
