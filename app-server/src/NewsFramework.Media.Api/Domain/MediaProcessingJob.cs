namespace NewsFramework.Media.Api.Domain;

public sealed class MediaProcessingJob
{
    public string JobId { get; init; } = string.Empty;
    public string MediaId { get; init; } = string.Empty;
    public string JobType { get; init; } = string.Empty;
    public MediaProcessingStatus Status { get; set; }
    public string Error { get; set; } = string.Empty;
    public long CreatedAtUnixSeconds { get; init; }
    public long UpdatedAtUnixSeconds { get; set; }
}
