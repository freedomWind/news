namespace NewsFramework.Media.Api.Storage;

public interface IMediaObjectStorage
{
    Task<long> SaveAsync(string objectKey, Stream source, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string objectKey, CancellationToken cancellationToken);
    Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken);
}
