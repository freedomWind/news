$ErrorActionPreference = "Stop"

$baseUrl = if ($args.Count -gt 0) { $args[0].TrimEnd("/") } else { "http://localhost:5234" }

$health = Invoke-RestMethod -Uri "$baseUrl/health"
if ($health.status -ne "ok") {
    throw "Media API health check failed."
}

$asset = Invoke-RestMethod -Uri "$baseUrl/api/media/med_sample_cover"
if ($asset.mediaId -ne "med_sample_cover" -or $asset.variants.Count -lt 1) {
    throw "Sample media metadata smoke check failed."
}

$content = Invoke-WebRequest -UseBasicParsing -Uri "$baseUrl/api/media/variants/var_sample_cover/content"
if ($content.StatusCode -ne 200 -or $content.Headers["Content-Type"] -notlike "image/png*") {
    throw "Sample media content smoke check failed."
}

Write-Output "Media API smoke checks passed: $baseUrl"
