using System.Text.Json;
using System.Text.Json.Serialization;
using NewsFramework.Media.Api.Domain;

namespace NewsFramework.Media.Api.Repositories;

public sealed class PersistentMediaRepository : IMediaRepository
{
    private readonly string statePath;
    private readonly SemaphoreSlim gate = new(1, 1);
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private MediaRepositoryState state;

    public PersistentMediaRepository(IConfiguration configuration, IHostEnvironment environment)
    {
        var configuredRoot = configuration["Media:MetadataRoot"];
        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            configuredRoot = Path.Combine("data", "media-metadata");
        }

        var rootPath = Path.IsPathRooted(configuredRoot)
            ? Path.GetFullPath(configuredRoot)
            : Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredRoot));

        Directory.CreateDirectory(rootPath);
        statePath = Path.Combine(rootPath, "repository.json");
        state = LoadState();
        EnsureSampleCoverSeed();
        SaveLockedAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task AddAssetAsync(MediaAsset asset, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            state.Assets[asset.MediaId] = asset;
            await SaveLockedAsync(cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<MediaAsset?> GetAssetAsync(string mediaId, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            return state.Assets.TryGetValue(mediaId, out var asset) ? asset : null;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<MediaUploadSession?> GetUploadSessionAsync(string uploadId, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            return state.UploadSessions.TryGetValue(uploadId, out var session) ? session : null;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<MediaVariant?> GetVariantAsync(string variantId, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            return state.Variants.TryGetValue(variantId, out var variant) ? variant : null;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task AddUploadSessionAsync(MediaUploadSession session, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            state.UploadSessions[session.UploadId] = session;
            await SaveLockedAsync(cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task AddVariantAsync(MediaVariant variant, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            state.Variants[variant.VariantId] = variant;
            AttachVariantLocked(variant);
            await SaveLockedAsync(cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task UpdateAssetAsync(MediaAsset asset, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            state.Assets[asset.MediaId] = asset;
            await SaveLockedAsync(cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task UpdateUploadSessionAsync(MediaUploadSession session, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            state.UploadSessions[session.UploadId] = session;
            await SaveLockedAsync(cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }

    private MediaRepositoryState LoadState()
    {
        if (!File.Exists(statePath))
        {
            return new MediaRepositoryState();
        }

        using var stream = File.OpenRead(statePath);
        var loaded = JsonSerializer.Deserialize<MediaRepositoryState>(stream, jsonOptions) ?? new MediaRepositoryState();
        RebuildVariantLists(loaded);
        return loaded;
    }

    private void EnsureSampleCoverSeed()
    {
        const string sampleMediaId = "med_sample_cover";
        const string sampleVariantId = "var_sample_cover";

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var createdAt = state.Assets.TryGetValue(sampleMediaId, out var existingAsset)
            ? existingAsset.CreatedAtUnixSeconds
            : now;

        var asset = new MediaAsset
        {
            MediaId = sampleMediaId,
            Type = MediaAssetType.Image,
            Status = MediaAssetStatus.Ready,
            FileName = "cover.png",
            ContentType = "image/png",
            FileSizeBytes = 68,
            OwnerId = "system",
            Purpose = "sample_cover",
            CreatedAtUnixSeconds = createdAt,
            UpdatedAtUnixSeconds = now
        };

        var variant = new MediaVariant
        {
            VariantId = sampleVariantId,
            MediaId = sampleMediaId,
            Kind = MediaVariantKind.Original,
            Status = MediaAssetStatus.Ready,
            ObjectKey = "med_sample_cover/original/cover.png",
            Url = "/api/media/variants/var_sample_cover/content",
            ContentType = "image/png",
            FileSizeBytes = 68,
            Width = 1,
            Height = 1,
            CreatedAtUnixSeconds = createdAt
        };

        state.Assets[sampleMediaId] = asset;
        state.Variants[sampleVariantId] = variant;
        AttachVariantLocked(variant);
        SaveLockedAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    private async Task SaveLockedAsync(CancellationToken cancellationToken)
    {
        var tempPath = statePath + ".tmp";
        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, state, jsonOptions, cancellationToken);
        }

        File.Move(tempPath, statePath, true);
    }

    private static void RebuildVariantLists(MediaRepositoryState loaded)
    {
        foreach (var asset in loaded.Assets.Values)
        {
            asset.Variants.Clear();
        }

        foreach (var variant in loaded.Variants.Values)
        {
            if (loaded.Assets.TryGetValue(variant.MediaId, out var asset))
            {
                asset.Variants.RemoveAll(existing => existing.VariantId == variant.VariantId);
                asset.Variants.Add(variant);
            }
        }
    }

    private void AttachVariantLocked(MediaVariant variant)
    {
        if (!state.Assets.TryGetValue(variant.MediaId, out var asset))
        {
            return;
        }

        asset.Variants.RemoveAll(existing => existing.VariantId == variant.VariantId);
        asset.Variants.Add(variant);
    }

    private sealed class MediaRepositoryState
    {
        public Dictionary<string, MediaAsset> Assets { get; init; } = new();
        public Dictionary<string, MediaUploadSession> UploadSessions { get; init; } = new();
        public Dictionary<string, MediaVariant> Variants { get; init; } = new();
    }
}
