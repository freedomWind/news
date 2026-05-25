# NewsFramework Baseline Import

## Source

```text
D:/_spaceWork/newsframework
```

## Target

```text
D:/_spaceWork/news/dev
```

## Import Strategy

The source demo is valuable as a complete Unity news + lightweight-game
baseline, so the import keeps source-level assets and documentation while
excluding generated output.

Included:

- `Assets/`
- `Packages/`
- `ProjectSettings/`
- `docs/`
- `app-server/` source, docs, scripts, and project files
- `_参考框架/`
- `设计图/`

Excluded:

- Unity generated folders: `Library/`, `Temp/`, `Logs/`, `UserSettings/`
- Unity generated root files: `*.csproj`, `*.sln`
- Backend generated folders: `.dotnet/`, `.buildcheck/`, `data/`, `bin/`, `obj/`
- Local verification logs and OS files

## Key Assets

- Data-driven UI pipeline: `Assets/Scripts/Runtime/UI/Blocks/`
- Page and feature UI: `Assets/Scripts/Runtime/UI/Pages/`,
  `Assets/Scripts/Runtime/UI/Features/`
- Content models and mock data: `Assets/Scripts/Runtime/Data/`
- Content HTTP/cache layer: `Assets/Scripts/Runtime/Services/Content/`
- Media client layer: `Assets/Scripts/Runtime/Services/Media/`
- Xiangqi/game runtime: `Assets/Scripts/Runtime/Chess/`,
  `Assets/Scripts/Runtime/Simulation/`, `Assets/Scripts/Runtime/Replay/`
- Backend MVP: `app-server/src/NewsFramework.Content.Api`,
  `app-server/src/NewsFramework.Media.Api`

## Verification

Backend:

```text
dotnet build .\app-server\NewsFramework.AppServer.sln
Result: 0 warnings, 0 errors
```

Unity:

```text
Unity 2022.3.61f1 batch import/compile
Result: process exit code 0
Log tail: Exiting batchmode successfully now

Unity 2022.3.61f1 SceneBuilder
Result: generated Assets/App/Scenes/HomeDemo.unity, process exit code 0
```

Notes:

- Unity log has startup licensing-client handshake errors before resolving
  entitlement. The editor then continues, compiles scripts, and exits
  successfully.
- `Assets/RuntimeResources/Scenes/HomeDemo.unity` is retained from the source
  baseline as a reference scene. `Assets/App/Scenes/HomeDemo.unity` is generated
  by `HomeDemoSceneBuilder` and is the forward project path.

## Follow-Up Architecture Adjustment

The target product should be news-media first, with UI generated from data by
default. The existing block architecture should be extended rather than
replaced:

- Keep `BlockData`/page schema as the upper-level content contract.
- Add renderer metadata such as `renderer`, `prefabKey`, `schemaVersion`, and
  fallback fields.
- Introduce a `RendererRegistry` that can select a procedural renderer or a
  prefab renderer.
- Introduce a prefab binding contract (`IDataBoundView` or `PrefabBinder`) so
  designer-owned prefabs bind to stable fields without owning business logic.
- Keep actions, routing, analytics, service refresh, and cache behavior in the
  unified pipeline.
- Prefer procedural rendering for feeds/articles; prefer prefab rendering for
  game rooms, complex functional pages, and campaign-style pages.
