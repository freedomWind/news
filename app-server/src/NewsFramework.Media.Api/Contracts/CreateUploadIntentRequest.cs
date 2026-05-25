namespace NewsFramework.Media.Api.Contracts;

public sealed class CreateUploadIntentRequest
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string MediaType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string Checksum { get; init; } = string.Empty;
    public string OwnerId { get; init; } = string.Empty;
    public string Purpose { get; init; } = string.Empty;
}
