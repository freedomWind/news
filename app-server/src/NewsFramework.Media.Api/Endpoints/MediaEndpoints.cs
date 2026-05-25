using NewsFramework.Media.Api.Contracts;
using NewsFramework.Media.Api.Domain;
using NewsFramework.Media.Api.Mapping;
using NewsFramework.Media.Api.Repositories;
using NewsFramework.Media.Api.Storage;
using NewsFramework.Media.Api.Utilities;

namespace NewsFramework.Media.Api.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/media");

        group.MapPost("/upload-intent", CreateUploadIntentAsync);
        group.MapPut("/uploads/{uploadId}/bytes", UploadBytesAsync);
        group.MapPost("/complete", CompleteUploadAsync);
        group.MapGet("/{mediaId}", GetMediaAsync);
        group.MapGet("/{mediaId}/variants", GetVariantsAsync);
        group.MapGet("/variants/{variantId}/content", GetVariantContentAsync);

        return routes;
    }

    private static async Task<IResult> CreateUploadIntentAsync(
        CreateUploadIntentRequest request,
        IMediaRepository repository,
        CancellationToken cancellationToken)
    {
        var validationError = MediaValidation.ValidateUploadIntent(request);
        if (validationError != null)
        {
            return Results.BadRequest(new ErrorResponse { Error = validationError });
        }

        MediaValidation.TryParseMediaType(request.MediaType, out var mediaType);
        var now = MediaClock.UnixSeconds();
        var mediaId = IdGenerator.NewId("med");
        var uploadId = IdGenerator.NewId("upl");
        var safeFileName = MediaValidation.SanitizeFileName(request.FileName);
        var objectKey = mediaId + "/original/" + safeFileName;
        var uploadUrl = "/api/media/uploads/" + uploadId + "/bytes";

        var asset = new MediaAsset
        {
            MediaId = mediaId,
            Type = mediaType,
            Status = MediaAssetStatus.AwaitingUpload,
            FileName = safeFileName,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            Checksum = request.Checksum,
            OwnerId = request.OwnerId,
            Purpose = request.Purpose,
            CreatedAtUnixSeconds = now,
            UpdatedAtUnixSeconds = now
        };

        var session = new MediaUploadSession
        {
            UploadId = uploadId,
            MediaId = mediaId,
            ObjectKey = objectKey,
            UploadUrl = uploadUrl,
            ExpiresAtUnixSeconds = now + 15 * 60,
            Status = MediaUploadStatus.Created
        };

        await repository.AddAssetAsync(asset, cancellationToken);
        await repository.AddUploadSessionAsync(session, cancellationToken);

        return Results.Ok(new UploadIntentResponse
        {
            MediaId = mediaId,
            UploadId = uploadId,
            UploadUrl = uploadUrl,
            Method = "PUT",
            ExpiresAtUnixSeconds = session.ExpiresAtUnixSeconds,
            MaxSizeBytes = MediaValidation.MaxUploadBytes,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", request.ContentType }
            }
        });
    }

    private static async Task<IResult> UploadBytesAsync(
        string uploadId,
        HttpRequest request,
        IMediaRepository repository,
        IMediaObjectStorage storage,
        CancellationToken cancellationToken)
    {
        var session = await repository.GetUploadSessionAsync(uploadId, cancellationToken);
        if (session == null)
        {
            return Results.NotFound(new ErrorResponse { Error = "upload session not found" });
        }

        if (session.ExpiresAtUnixSeconds < MediaClock.UnixSeconds())
        {
            session.Status = MediaUploadStatus.Expired;
            await repository.UpdateUploadSessionAsync(session, cancellationToken);
            return Results.BadRequest(new ErrorResponse { Error = "upload session expired" });
        }

        var bytes = await storage.SaveAsync(session.ObjectKey, request.Body, cancellationToken);
        session.Status = MediaUploadStatus.BytesReceived;
        await repository.UpdateUploadSessionAsync(session, cancellationToken);

        return Results.Ok(new UploadBytesResponse
        {
            UploadId = session.UploadId,
            BytesReceived = bytes
        });
    }

    private static async Task<IResult> CompleteUploadAsync(
        CompleteUploadRequest request,
        IMediaRepository repository,
        IMediaObjectStorage storage,
        CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.UploadId))
        {
            return Results.BadRequest(new ErrorResponse { Error = "uploadId is required" });
        }

        var session = await repository.GetUploadSessionAsync(request.UploadId, cancellationToken);
        if (session == null)
        {
            return Results.NotFound(new ErrorResponse { Error = "upload session not found" });
        }

        var asset = await repository.GetAssetAsync(session.MediaId, cancellationToken);
        if (asset == null)
        {
            return Results.NotFound(new ErrorResponse { Error = "media asset not found" });
        }

        if (!await storage.ExistsAsync(session.ObjectKey, cancellationToken))
        {
            return Results.BadRequest(new ErrorResponse { Error = "upload bytes are missing" });
        }

        var now = MediaClock.UnixSeconds();
        session.Status = MediaUploadStatus.Completed;
        asset.Status = MediaAssetStatus.Ready;
        asset.FileSizeBytes = request.FileSizeBytes > 0 ? request.FileSizeBytes : asset.FileSizeBytes;
        asset.Checksum = string.IsNullOrWhiteSpace(request.Checksum) ? asset.Checksum : request.Checksum;
        asset.UpdatedAtUnixSeconds = now;

        var original = new MediaVariant
        {
            VariantId = IdGenerator.NewId("var"),
            MediaId = asset.MediaId,
            Kind = MediaVariantKind.Original,
            Status = MediaAssetStatus.Ready,
            ObjectKey = session.ObjectKey,
            Url = "/api/media/variants/",
            ContentType = asset.ContentType,
            FileSizeBytes = asset.FileSizeBytes,
            CreatedAtUnixSeconds = now
        };
        original.Url += original.VariantId + "/content";

        await repository.UpdateUploadSessionAsync(session, cancellationToken);
        await repository.UpdateAssetAsync(asset, cancellationToken);
        await repository.AddVariantAsync(original, cancellationToken);

        return Results.Ok(MediaDtoMapper.ToDto(asset));
    }

    private static async Task<IResult> GetMediaAsync(
        string mediaId,
        IMediaRepository repository,
        CancellationToken cancellationToken)
    {
        var asset = await repository.GetAssetAsync(mediaId, cancellationToken);
        return asset == null
            ? Results.NotFound(new ErrorResponse { Error = "media asset not found" })
            : Results.Ok(MediaDtoMapper.ToDto(asset));
    }

    private static async Task<IResult> GetVariantsAsync(
        string mediaId,
        IMediaRepository repository,
        CancellationToken cancellationToken)
    {
        var asset = await repository.GetAssetAsync(mediaId, cancellationToken);
        return asset == null
            ? Results.NotFound(new ErrorResponse { Error = "media asset not found" })
            : Results.Ok(asset.Variants.Select(MediaDtoMapper.ToDto).ToArray());
    }

    private static async Task<IResult> GetVariantContentAsync(
        string variantId,
        IMediaRepository repository,
        IMediaObjectStorage storage,
        CancellationToken cancellationToken)
    {
        var variant = await repository.GetVariantAsync(variantId, cancellationToken);
        if (variant == null)
        {
            return Results.NotFound(new ErrorResponse { Error = "media variant not found" });
        }

        var stream = await storage.OpenReadAsync(variant.ObjectKey, cancellationToken);
        if (stream == null)
        {
            return Results.NotFound(new ErrorResponse { Error = "media object not found" });
        }

        return Results.File(stream, variant.ContentType);
    }
}
