namespace NewsFramework.Media.Api.Domain;

public sealed class MediaUploadSession
{
    public string UploadId { get; init; } = string.Empty;
    public string MediaId { get; init; } = string.Empty;
    public string ObjectKey { get; init; } = string.Empty;
    public string UploadUrl { get; set; } = string.Empty;
    public long ExpiresAtUnixSeconds { get; init; }
    public MediaUploadStatus Status { get; set; }
}
