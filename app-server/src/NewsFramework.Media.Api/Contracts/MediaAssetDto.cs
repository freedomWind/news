namespace NewsFramework.Media.Api.Contracts;

public sealed class MediaAssetDto
{
    public string MediaId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string Checksum { get; init; } = string.Empty;
    public string OwnerId { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
    public long CreatedAtUnixSeconds { get; init; }
    public long UpdatedAtUnixSeconds { get; init; }
    public IReadOnlyList<MediaVariantDto> Variants { get; init; } = Array.Empty<MediaVariantDto>();
}
