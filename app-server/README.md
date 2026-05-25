# NewsFramework App Server

This directory is the independent backend workspace for the app server.

Current scope:

- Media service MVP is implemented.
- Content service MVP is implemented with in-memory sample content.
- No game server logic.
- No frame sync, match room, chess rules, or replay simulation service.

## Projects

```text
src/NewsFramework.Media.Api
src/NewsFramework.Content.Api
```

See:

- `docs/server-goals.md`
- `docs/media-server-design.md`
- `docs/content-server-design.md`
- `docs/media-api-contract.md`

## Run

Media API:

```powershell
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
dotnet run --project src/NewsFramework.Media.Api --urls http://localhost:5234
```

Content API:

```powershell
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
dotnet run --project src/NewsFramework.Content.Api --urls http://localhost:5235
```

Default local URLs:

```text
Media API:   http://localhost:5234
Content API: http://localhost:5235
```

## Build

```powershell
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
dotnet build NewsFramework.AppServer.sln
```

## Smoke

With the APIs running:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\smoke-media.ps1
powershell -ExecutionPolicy Bypass -File .\scripts\smoke-media-upload.ps1
powershell -ExecutionPolicy Bypass -File .\scripts\smoke-content.ps1
```

## API Shape

### Media API

The media module is designed around direct-upload semantics:

```text
POST /api/media/upload-intent
PUT  /api/media/uploads/{uploadId}/bytes
POST /api/media/complete
GET  /api/media/{mediaId}
GET  /api/media/{mediaId}/variants
GET  /api/media/variants/{variantId}/content
```

The local MVP stores files under `data/media-objects/` relative to the Media API content root.
Media metadata is persisted as local JSON under `data/media-metadata/` relative to the Media API content root.
Production should replace local storage with object storage and CDN URLs while keeping the same public contract.

### Content API

```text
GET /api/feed/home?cursor=&limit=&knownVersion=
GET /api/articles/{articleId}?knownVersion=
GET /api/articles/{articleId}/engagement-summary
```

Content responses should align with Unity's current `FeedPageResult`, `PageData`, `BlockData`, and `ArticleEngagementSummaryData` contracts.

Content data references media by `mediaId` and URLs returned by the media service. It does not store media binaries.
The current MVP uses in-memory sample content; production should replace it with PostgreSQL or a CMS adapter behind `Content.Api`.

## Agent Boundaries

Future agents can work independently by area:

- Media API contract and Unity client integration.
- Content API contract and Unity client integration.
- Media storage adapter: local -> S3-compatible object storage.
- Metadata persistence: in-memory -> PostgreSQL.
- Media processing worker: image thumbnails and video transcode jobs.
- Auth and upload/content permission policy.

Do not add game server concepts to this workspace until the game service is explicitly started.
Do not bind Unity directly to a CMS native API. If a CMS is introduced, expose it through `Content.Api`.
