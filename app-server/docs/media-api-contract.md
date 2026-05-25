# Media API Contract

## Create Upload Intent

```http
POST /api/media/upload-intent
Content-Type: application/json
```

Request:

```json
{
  "fileName": "cover.jpg",
  "contentType": "image/jpeg",
  "mediaType": "image",
  "fileSizeBytes": 123456,
  "checksum": "",
  "ownerId": "user_001",
  "purpose": "article_cover"
}
```

Response:

```json
{
  "mediaId": "med_xxx",
  "uploadId": "upl_xxx",
  "uploadUrl": "/api/media/uploads/upl_xxx/bytes",
  "method": "PUT",
  "expiresAtUnixSeconds": 1770000000,
  "maxSizeBytes": 52428800,
  "headers": {
    "Content-Type": "image/jpeg"
  }
}
```

## Upload Bytes

Local MVP endpoint:

```http
PUT /api/media/uploads/{uploadId}/bytes
Content-Type: image/jpeg
```

Production may replace this with direct object storage upload.

## Complete Upload

```http
POST /api/media/complete
Content-Type: application/json
```

Request:

```json
{
  "uploadId": "upl_xxx",
  "fileSizeBytes": 123456,
  "checksum": ""
}
```

Response is `MediaAssetDto`.

## Get Asset

```http
GET /api/media/{mediaId}
```

Response:

```json
{
  "mediaId": "med_xxx",
  "type": "image",
  "status": "ready",
  "fileName": "cover.jpg",
  "contentType": "image/jpeg",
  "fileSizeBytes": 123456,
  "checksum": "",
  "ownerId": "user_001",
  "purpose": "article_cover",
  "createdAtUnixSeconds": 1770000000,
  "updatedAtUnixSeconds": 1770000001,
  "variants": [
    {
      "variantId": "var_xxx",
      "mediaId": "med_xxx",
      "kind": "original",
      "status": "ready",
      "url": "/api/media/variants/var_xxx/content",
      "contentType": "image/jpeg",
      "fileSizeBytes": 123456,
      "width": 0,
      "height": 0,
      "durationSeconds": 0
    }
  ]
}
```
