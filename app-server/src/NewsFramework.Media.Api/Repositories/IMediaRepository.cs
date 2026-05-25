using NewsFramework.Media.Api.Domain;

namespace NewsFramework.Media.Api.Repositories;

public interface IMediaRepository
{
    Task AddAssetAsync(MediaAsset asset, CancellationToken cancellationToken);
    Task<MediaAsset?> GetAssetAsync(string mediaId, CancellationToken cancellationToken);
    Task<MediaUploadSession?> GetUploadSessionAsync(string uploadId, CancellationToken cancellationToken);
    Task<MediaVariant?> GetVariantAsync(string variantId, CancellationToken cancellationToken);
    Task AddUploadSessionAsync(MediaUploadSession session, CancellationToken cancellationToken);
    Task AddVariantAsync(MediaVariant variant, CancellationToken cancellationToken);
    Task UpdateAssetAsync(MediaAsset asset, CancellationToken cancellationToken);
    Task UpdateUploadSessionAsync(MediaUploadSession session, CancellationToken cancellationToken);
}
