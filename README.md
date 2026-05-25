# News

Unity-based short-news and lightweight game app baseline.

This repository was initialized from `D:/_spaceWork/newsframework` as a
working baseline for a data-driven news media app with Unity UI and lightweight
game modules.

## Imported Value

- Unity 2022.3.61f1 project settings and package manifest.
- Runtime UI framework built around `BlockData`, `BlockRegistry`, and
  `BlockView` implementations.
- News/feed/article data models, mock data, HTTP content service, SQLite cache,
  and media service client code.
- Xiangqi simulation and replay runtime skeleton for lightweight game content.
- ASP.NET Core app server MVP with Content API and Media API.
- Product, UI architecture, schema, API, and acceptance documentation.
- Design reference HTML/images under `设计图/`.

Generated folders and local caches are intentionally not tracked:
`Library/`, `Temp/`, `Logs/`, `UserSettings/`, root Unity `.csproj/.sln`,
and app-server `bin/obj`.

## Unity

Open this folder with Unity `2022.3.61f1`.

Primary generated demo scene:

```text
Assets/App/Scenes/HomeDemo.unity
```

The imported source baseline also keeps `Assets/RuntimeResources/Scenes/HomeDemo.unity`
for reference.

Batch import/compile verification:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\Unity 2022.3.61f1\Editor\Unity.exe' `
  -batchmode -quit -nographics `
  -projectPath 'D:\_spaceWork\news\dev' `
  -logFile 'D:\_spaceWork\news\dev\unity-build.log'
```

## App Server

```powershell
cd D:\_spaceWork\news\dev\app-server
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
dotnet build .\NewsFramework.AppServer.sln
```

Run APIs:

```powershell
dotnet run --project src/NewsFramework.Media.Api --urls http://localhost:5234
dotnet run --project src/NewsFramework.Content.Api --urls http://localhost:5235
```

Smoke scripts are in `app-server/scripts/`.

## Next Architecture Step

The current baseline is mostly programmatic data-driven UI. For the target
product, keep the unified schema as the source of truth and add a hybrid
rendering path:

- Procedural renderer for news feeds, article details, and structured media
  content.
- Prefab renderer for game rooms, complex functional pages, and designer-owned
  visual modules.
- `RendererRegistry` should choose procedural or prefab rendering per block/page.
- Prefabs should implement a stable binding contract, such as
  `IDataBoundView`/`PrefabBinder`, so prefab assets remain presentation only.
- Data should carry `prefabKey`, schema version, resource keys, actions, and
  fallback information; business routing, analytics, refresh, and service calls
  should remain in the unified rendering pipeline.
