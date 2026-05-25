namespace NewsFramework.Media.Api.Contracts;

public sealed class UploadIntentResponse
{
    public string MediaId { get; init; } = string.Empty;
    public string UploadId { get; init; } = string.Empty;
    public string UploadUrl { get; init; } = string.Empty;
    public string Method { get; init; } = "PUT";
    public long ExpiresAtUnixSeconds { get; init; }
    public long MaxSizeBytes { get; init; }
    public IReadOnlyDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
}
