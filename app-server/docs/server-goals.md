# App Server Goals

## Current Goal

Build the app backend as independent modules under `app-server`.

The server should support the Unity client without coupling Unity to implementation details such as ORM models, object storage providers, or CMS-native APIs.

## Current Modules

### Media Service

Status: MVP implemented.

Responsibilities:

- upload intent
- local upload bytes endpoint for MVP
- complete upload
- media asset metadata
- media variants
- local object storage adapter

Production replacements:

- PostgreSQL metadata repository
- S3-compatible object storage
- CDN URL generation
- media processing workers

### Content Service

Status: MVP implemented with in-memory sample content.

Responsibilities:

- home feed pages
- article detail pages
- block schema JSON for Unity
- article engagement summary
- media references inside content blocks

The content service should return Unity-oriented contracts, not CMS-native contracts.

Production replacements:

- PostgreSQL or CMS-backed content repository
- published/draft content workflow
- feed ranking and pagination source
- comment and engagement persistence

### Game Service

Status: out of scope.

Do not implement:

- game rooms
- chess rules
- frame sync
- replay simulation
- matchmaking

## Language Direction

Use C# / ASP.NET Core for the current server workspace.

Reasons:

- Unity client is C#.
- DTO and wire contracts are easier to keep aligned.
- Current media MVP is already ASP.NET Core.
- Future Java migration remains possible if API contracts and database schemas stay implementation-neutral.

## CMS Policy

Do not fork a large CMS as the app server.

Allowed future integration:

```text
Unity -> Content.Api -> CMS/PostgreSQL
```

Disallowed:

```text
Unity -> CMS native API
```

The app server should own the public contract consumed by Unity.

## Agent Boundaries

Server agents working in `app-server` should not modify Unity client files.

Unity agents should not modify server implementation files unless doing an explicit integration task.

Shared contracts should be documented in `app-server/docs/` and mirrored in Unity only through stable DTOs or generated clients.
