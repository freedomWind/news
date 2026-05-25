using NewsFramework.Media.Api.Contracts;
using NewsFramework.Media.Api.Domain;

namespace NewsFramework.Media.Api.Mapping;

public static class MediaDtoMapper
{
    public static MediaAssetDto ToDto(MediaAsset asset)
    {
        return new MediaAssetDto
        {
            MediaId = asset.MediaId,
            Type = ToWireName(asset.Type),
            Status = ToWireName(asset.Status),
            FileName = asset.FileName,
            ContentType = asset.ContentType,
            FileSizeBytes = asset.FileSizeBytes,
            Checksum = asset.Checksum,
            OwnerId = asset.OwnerId,
            Purpose = asset.Purpose,
            CreatedAtUnixSeconds = asset.CreatedAtUnixSeconds,
            UpdatedAtUnixSeconds = asset.UpdatedAtUnixSeconds,
            Variants = asset.Variants.Select(ToDto).ToArray()
        };
    }

    public static MediaVariantDto ToDto(MediaVariant variant)
    {
        return new MediaVariantDto
        {
            VariantId = variant.VariantId,
            MediaId = variant.MediaId,
            Kind = ToWireName(variant.Kind),
            Status = ToWireName(variant.Status),
            Url = variant.Url,
            ContentType = variant.ContentType,
            FileSizeBytes = variant.FileSizeBytes,
            Width = variant.Width,
            Height = variant.Height,
            DurationSeconds = variant.DurationSeconds
        };
    }

    private static string ToWireName(Enum value)
    {
        var name = value.ToString();
        return string.Concat(name.Select((character, index) =>
            index > 0 && char.IsUpper(character)
                ? "_" + char.ToLowerInvariant(character)
                : char.ToLowerInvariant(character).ToString()));
    }
}
