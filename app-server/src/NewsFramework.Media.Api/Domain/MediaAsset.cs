using System.Text.Json.Serialization;

namespace NewsFramework.Media.Api.Domain;

public sealed class MediaAsset
{
    public string MediaId { get; init; } = string.Empty;
    public MediaAssetType Type { get; set; }
    public MediaAssetStatus Status { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public long CreatedAtUnixSeconds { get; init; }
    public long UpdatedAtUnixSeconds { get; set; }
    [JsonIgnore]
    public List<MediaVariant> Variants { get; } = new();
}
