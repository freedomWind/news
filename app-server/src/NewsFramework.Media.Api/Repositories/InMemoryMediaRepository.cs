using System.Collections.Concurrent;
using NewsFramework.Media.Api.Domain;

namespace NewsFramework.Media.Api.Repositories;

public sealed class InMemoryMediaRepository : IMediaRepository
{
    private readonly ConcurrentDictionary<string, MediaAsset> assets = new();
    private readonly ConcurrentDictionary<string, MediaUploadSession> uploadSessions = new();
    private readonly ConcurrentDictionary<string, MediaVariant> variants = new();

    public Task AddAssetAsync(MediaAsset asset, CancellationToken cancellationToken)
    {
        assets[asset.MediaId] = asset;
        return Task.CompletedTask;
    }

    public Task<MediaAsset?> GetAssetAsync(string mediaId, CancellationToken cancellationToken)
    {
        assets.TryGetValue(mediaId, out var asset);
        return Task.FromResult(asset);
    }

    public Task<MediaUploadSession?> GetUploadSessionAsync(string uploadId, CancellationToken cancellationToken)
    {
        uploadSessions.TryGetValue(uploadId, out var session);
        return Task.FromResult(session);
    }

    public Task<MediaVariant?> GetVariantAsync(string variantId, CancellationToken cancellationToken)
    {
        variants.TryGetValue(variantId, out var variant);
        return Task.FromResult(variant);
    }

    public Task AddUploadSessionAsync(MediaUploadSession session, CancellationToken cancellationToken)
    {
        uploadSessions[session.UploadId] = session;
        return Task.CompletedTask;
    }

    public Task AddVariantAsync(MediaVariant variant, CancellationToken cancellationToken)
    {
        variants[variant.VariantId] = variant;
        if (assets.TryGetValue(variant.MediaId, out var asset))
        {
            lock (asset.Variants)
            {
                asset.Variants.RemoveAll(existing => existing.VariantId == variant.VariantId);
                asset.Variants.Add(variant);
            }
        }

        return Task.CompletedTask;
    }

    public Task UpdateAssetAsync(MediaAsset asset, CancellationToken cancellationToken)
    {
        assets[asset.MediaId] = asset;
        return Task.CompletedTask;
    }

    public Task UpdateUploadSessionAsync(MediaUploadSession session, CancellationToken cancellationToken)
    {
        uploadSessions[session.UploadId] = session;
        return Task.CompletedTask;
    }
}
