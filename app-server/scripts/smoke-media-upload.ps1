$ErrorActionPreference = "Stop"

$baseUrl = if ($args.Count -gt 0) { $args[0].TrimEnd("/") } else { "http://localhost:5234" }

$pngBytes = [Convert]::FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=")
$fileName = "smoke-upload-" + [DateTimeOffset]::UtcNow.ToUnixTimeSeconds() + ".png"

$intentBody = @{
    fileName = $fileName
    contentType = "image/png"
    mediaType = "image"
    fileSizeBytes = $pngBytes.Length
    checksum = ""
    ownerId = "smoke"
    purpose = "upload_regression"
} | ConvertTo-Json -Compress

$intent = Invoke-RestMethod `
    -Method Post `
    -Uri "$baseUrl/api/media/upload-intent" `
    -ContentType "application/json" `
    -Body $intentBody

$upload = Invoke-RestMethod `
    -Method Put `
    -Uri ($baseUrl + $intent.uploadUrl) `
    -ContentType "image/png" `
    -Body $pngBytes

if ($upload.bytesReceived -ne $pngBytes.Length) {
    throw "Upload bytes check failed. Expected $($pngBytes.Length), got $($upload.bytesReceived)."
}

$completeBody = @{
    uploadId = $intent.uploadId
    fileSizeBytes = $pngBytes.Length
    checksum = ""
} | ConvertTo-Json -Compress

$asset = Invoke-RestMethod `
    -Method Post `
    -Uri "$baseUrl/api/media/complete" `
    -ContentType "application/json" `
    -Body $completeBody

if ($asset.mediaId -ne $intent.mediaId -or $asset.status -ne "ready" -or $asset.variants.Count -lt 1) {
    throw "Complete upload smoke check failed."
}

$variant = $asset.variants[0]
$content = Invoke-WebRequest -UseBasicParsing -Uri ($baseUrl + $variant.url)
if ($content.StatusCode -ne 200 -or $content.Headers["Content-Type"] -notlike "image/png*") {
    throw "Uploaded media content smoke check failed."
}

[pscustomobject]@{
    MediaId = $asset.mediaId
    VariantId = $variant.variantId
    Url = $variant.url
    Bytes = $content.RawContentStream.Length
} | ConvertTo-Json -Compress
