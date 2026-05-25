# Content Server Design

## Goal

Build a `Content.Api` service that provides app-specific content contracts for Unity.

It owns:

- home feed page assembly
- article detail payloads
- block schema JSON
- article engagement summary
- references to media assets

It does not own:

- media binaries
- object storage
- upload sessions
- game logic
- frame sync

## Current MVP

`src/NewsFramework.Content.Api` implements the first MVP with an in-memory content repository.

Implemented API:

```http
GET /api/feed/home?cursor=&limit=&knownVersion=
GET /api/articles/{articleId}?knownVersion=
GET /api/articles/{articleId}/engagement-summary
```

## Unity Contract Alignment

The API should align with the current Unity client shapes:

- `FeedPageResult`
- `PageData`
- `BlockData`
- `MediaAssetData`
- `ArticleEngagementSummaryData`

The server can have internal models, but the HTTP response should be Unity-oriented.

## Feed Page Response

The home feed endpoint should return:

```json
{
  "success": true,
  "data": {
    "feedId": "home",
    "title": "今日棋坛",
    "feedVersion": "home_v1",
    "cursor": "",
    "nextCursor": "6",
    "hasMore": true,
    "estimatedTotal": 100,
    "expiresInSeconds": 120,
    "blocks": []
  }
}
```

The `blocks` array should contain `BlockData`-compatible JSON.

## Article Detail Response

The article endpoint should return:

```json
{
  "success": true,
  "data": {
    "pageId": "article_001",
    "pageType": "article",
    "title": "Article title",
    "version": "v1",
    "expiresInSeconds": 86400,
    "blocks": []
  }
}
```

Article content can include paragraphs, images, videos, replay blocks, board previews, and other content blocks already supported by Unity.

## Article Engagement Summary Response

The engagement endpoint should return:

```json
{
  "articleId": "article_001",
  "bookmarked": false,
  "flowered": false,
  "canComment": true,
  "flowerCount": 128,
  "commentCount": 8,
  "previewComments": []
}
```

This endpoint should provide the lightweight detail-page interaction summary in one request.
Full comment list pagination should be a separate future endpoint.

## Media References

Content blocks should reference media using `MediaAssetData`:

```json
{
  "mediaId": "med_xxx",
  "type": "image",
  "url": "/api/media/variants/var_xxx/content",
  "mimeType": "image/jpeg",
  "version": "v1",
  "width": 480,
  "height": 360,
  "aspectRatio": 1.33
}
```

The Unity client resolves relative media URLs through `MediaServerConfig`.

## CMS Integration Policy

If a CMS is introduced later, it should sit behind `Content.Api`.

```text
Unity -> Content.Api -> CMS/PostgreSQL
```

`Content.Api` should transform CMS-native data into Unity's stable content contracts.

## First MVP

1. Create `src/NewsFramework.Content.Api`. Done.
2. Implement in-memory/mock content repository. Done.
3. Implement home feed endpoint. Done.
4. Implement article detail endpoint. Done.
5. Implement article engagement summary endpoint. Done.
6. Add smoke tests with sample JSON responses. Done.
7. Keep media binary access delegated to `Media.Api`. Done.

## Next Replacement Points

1. Replace `InMemoryContentRepository` with PostgreSQL or a CMS-backed adapter.
2. Persist article engagement state.
3. Connect sample media references to persisted `Media.Api` records.
4. Add integration tests for feed, article detail, and engagement payloads.
