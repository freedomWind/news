namespace NewsFramework.Media.Api.Contracts;

public sealed class CompleteUploadRequest
{
    public string UploadId { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string Checksum { get; init; } = string.Empty;
}
