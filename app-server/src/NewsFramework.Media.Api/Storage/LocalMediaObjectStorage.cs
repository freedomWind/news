namespace NewsFramework.Media.Api.Storage;

public sealed class LocalMediaObjectStorage : IMediaObjectStorage
{
    private readonly string rootPath;

    public LocalMediaObjectStorage(IConfiguration configuration, IHostEnvironment environment)
    {
        var configuredRoot = configuration["Media:StorageRoot"];
        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            configuredRoot = Path.Combine("data", "media-objects");
        }

        rootPath = Path.IsPathRooted(configuredRoot)
            ? Path.GetFullPath(configuredRoot)
            : Path.GetFullPath(Path.Combine(environment.ContentRootPath, configuredRoot));
        Directory.CreateDirectory(rootPath);
    }

    public async Task<long> SaveAsync(string objectKey, Stream source, CancellationToken cancellationToken)
    {
        var path = ResolvePath(objectKey);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var target = File.Create(path);
        await source.CopyToAsync(target, cancellationToken);
        return target.Length;
    }

    public Task<bool> ExistsAsync(string objectKey, CancellationToken cancellationToken)
    {
        return Task.FromResult(File.Exists(ResolvePath(objectKey)));
    }

    public Task<Stream?> OpenReadAsync(string objectKey, CancellationToken cancellationToken)
    {
        var path = ResolvePath(objectKey);
        if (!File.Exists(path))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = File.OpenRead(path);
        return Task.FromResult<Stream?>(stream);
    }

    private string ResolvePath(string objectKey)
    {
        var safeKey = objectKey.Replace('\\', '/').TrimStart('/');
        var combined = Path.GetFullPath(Path.Combine(rootPath, safeKey));
        var root = Path.GetFullPath(rootPath);
        if (!combined.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Object key escapes storage root.");
        }

        return combined;
    }
}
