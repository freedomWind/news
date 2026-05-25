using NewsFramework.Media.Api.Contracts;
using NewsFramework.Media.Api.Domain;

namespace NewsFramework.Media.Api.Utilities;

public static class MediaValidation
{
    public const long MaxUploadBytes = 50L * 1024L * 1024L;

    public static bool TryParseMediaType(string value, out MediaAssetType type)
    {
        switch ((value ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "image":
                type = MediaAssetType.Image;
                return true;
            case "video":
                type = MediaAssetType.Video;
                return true;
            case "audio":
                type = MediaAssetType.Audio;
                return true;
            case "file":
                type = MediaAssetType.File;
                return true;
            default:
                type = MediaAssetType.File;
                return false;
        }
    }

    public static string? ValidateUploadIntent(CreateUploadIntentRequest request)
    {
        if (request == null)
        {
            return "request is required";
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return "fileName is required";
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            return "contentType is required";
        }

        if (!TryParseMediaType(request.MediaType, out _))
        {
            return "mediaType must be image, video, audio, or file";
        }

        if (request.FileSizeBytes < 0 || request.FileSizeBytes > MaxUploadBytes)
        {
            return "fileSizeBytes exceeds max upload size";
        }

        return null;
    }

    public static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(fileName.Select(character =>
            invalidChars.Contains(character) ? '_' : character).ToArray());
        return string.IsNullOrWhiteSpace(safe) ? "upload.bin" : safe;
    }
}
