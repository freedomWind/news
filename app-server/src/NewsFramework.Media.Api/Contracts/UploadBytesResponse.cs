namespace NewsFramework.Media.Api.Contracts;

public sealed class UploadBytesResponse
{
    public string UploadId { get; init; } = string.Empty;
    public long BytesReceived { get; init; }
}
