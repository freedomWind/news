# Media Server Design

## Goal

Build a standalone media service for the Unity app.

It owns:

- upload sessions
- media asset metadata
- variants such as original, thumbnail, poster, transcode outputs
- processing job records
- file/object URLs

It does not own:

- article content blocks
- feed pagination
- comments
- game rooms
- frame sync
- chess rules

## Architecture

```text
Unity Client
  -> Media HTTP API
      -> Media metadata repository
      -> Object storage adapter
      -> Processing job queue
      -> CDN/object URLs
```

Current MVP:

```text
ASP.NET Core Minimal API
  -> PersistentMediaRepository
  -> LocalMediaObjectStorage
```

The local MVP persists media metadata to JSON under `data/media-metadata/` relative to the Media API content root.
It seeds `med_sample_cover` / `var_sample_cover` so Content API sample feed media URLs resolve through Media API.

Production target:

```text
ASP.NET Core Minimal API
  -> PostgreSQL
  -> S3-compatible object storage
  -> CDN
  -> Queue
  -> FFmpeg/image worker
```

## Service Boundary

The content service should reference media by `mediaId` and optional variant metadata.
It should not store media binary data.

The Unity client should treat returned media URLs as opaque. In production they may be CDN URLs, signed object-store URLs, or API proxy URLs.

## Data Model

### media_assets

- `media_id`
- `type`: image, video, audio, file
- `status`: awaiting_upload, uploaded, processing, ready, failed, deleted
- `file_name`
- `content_type`
- `file_size_bytes`
- `checksum`
- `owner_id`
- `purpose`
- `created_at_unix_seconds`
- `updated_at_unix_seconds`

### media_variants

- `variant_id`
- `media_id`
- `kind`: original, thumbnail, poster, preview, transcode
- `status`
- `object_key`
- `url`
- `content_type`
- `file_size_bytes`
- `width`
- `height`
- `duration_seconds`
- `created_at_unix_seconds`

### media_upload_sessions

- `upload_id`
- `media_id`
- `object_key`
- `upload_url`
- `expires_at_unix_seconds`
- `status`

### media_processing_jobs

- `job_id`
- `media_id`
- `job_type`
- `status`
- `error`
- `created_at_unix_seconds`
- `updated_at_unix_seconds`

## First Production Replacement Points

1. Replace `PersistentMediaRepository` with PostgreSQL.
2. Replace `LocalMediaObjectStorage` with S3-compatible storage.
3. Add background workers for image thumbnails and video transcodes.
4. Add auth and ownership checks.
5. Add CDN URL generation.
