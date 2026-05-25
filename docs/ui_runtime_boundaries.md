# UI Runtime Boundaries

## Purpose

This document defines the runtime boundaries for the short-news + lightweight
game app. The goal is to keep the app organized as it grows from news feeds and
article details into game surfaces and feature pages.

Data-driven UI means:

- unified page and action contracts;
- unified lifecycle, navigation, analytics, resource loading, and fallback rules;
- multiple rendering domains chosen by registry, not one universal renderer.

It does not mean every screen must be assembled from the same `BlockData` model.

Related documents:

- `ui_architecture.md`: current Unity UI layers and existing runtime structure.
- `ui_composition_rules.md`: page, renderer, view, controller ownership rules.
- `block_schema.md`: content block schema for feeds and article details.
- `feature_page_schema.md`: feature page data contract.
- `game_room_schema.md`: current game room runtime data contract.
- `game_runtime_contract.md`: game entry, session lifecycle, view-state, input,
  result, resource, error, and fallback contract.

## Layer Map

```text
AppShell
  startup, app lifecycle, global overlays, tabs, safe area, analytics entry

Router / PageHost
  route table, navigation stack, page lifecycle, page mounting

PageController
  FeedPageController
  ArticlePageController
  FeaturePageController
  GameHostController

Renderer Registry
  BlockRegistry
  FeatureSectionRegistry
  future PrefabRendererRegistry
  future GameSurfaceRegistry

Renderer Domain
  Content Block Renderer
  Feature Page Renderer
  Prefab Renderer
  Game Surface Renderer

Data Contract
  PageData / BlockData / BlockActionData
  FeaturePageData / FeatureSectionData
  GameRoomData / future GameViewState

Services
  Content, Feed, Media, Game, Cache, Analytics, Resource
```

The top layers are stable orchestration. The bottom rendering domains may differ
per product area.

## Ownership Boundaries

| Layer | Owns | Must not own |
| --- | --- | --- |
| `AppShell` | startup, global lifecycle, tabs, overlays, safe area, analytics entry | block layout, article body rendering, game board rules |
| `Router/PageHost` | route table, navigation stack, page creation, enter/exit | network calls, view internals, game state mutation |
| `PageController` | page state, data loading, renderer selection, action forwarding | low-level UI layout, direct child view mutation |
| `Renderer` | creating/reusing view nodes from data, binding callbacks | service calls, navigation decisions, global state writes |
| `View/Prefab` | visual hierarchy, local input, local async asset callback | routing, API calls, analytics policy, game rules |
| `ActionDispatcher` | mapping actions to route/service/analytics work | rendering, direct UI mutation |
| `Service` | data, cache, network, persistence, domain API | Unity visual hierarchy |
| `GameSurface` | game visual state rendering and input forwarding | feed/article schema, app-level navigation policy |
| `GameLogic/Simulation` | rules, state transition, snapshot, frame/tick decisions | Unity UI rendering |

Hard rule: visual code emits actions; it does not decide app flow.

## Page Categories

### FeedPage

Used for the home feed, channel feeds, search results, and recommendation lists.

Contract:

```text
FeedPageController
  -> IFeedPager
  -> FeedPageResult.blocks
  -> BlockRenderer
  -> BlockActionData
```

Renderer:

- `BlockRenderer`
- `BlockRegistry`
- `BlockViewBase`

Allowed content:

- article cards;
- media cards;
- lightweight game entry cards;
- replay previews;
- list separators, headers, and spacers.

Not allowed:

- full game rooms;
- core game board/HUD;
- profile/settings page bodies.

### ArticlePage

Used for article details and long-form content.

Contract:

```text
ArticlePageController
  -> IContentService
  -> PageData.blocks
  -> BlockRenderer
  -> Article page-level engagement modules
```

Renderer:

- article body uses `BlockRenderer`;
- article engagement, comments, share, collect, and bottom input bar are
  page-level modules, not article body blocks.

Allowed content:

- article header;
- paragraph, image, video;
- embedded replay block;
- related article/game entry blocks.

Not allowed:

- comment pagination inside `BlockData`;
- global share/collect state inside body blocks;
- full game room runtime inside article body.

### GamePage / GameSurface

Used after the user enters a playable or watchable game context.

Detailed game runtime contracts are defined in `game_runtime_contract.md`.

Contract:

```text
GameHostController
  -> GameSessionDescriptor
  -> Game service / simulation / room state
  -> GameViewState or GameRoomData
  -> GameSurfaceRenderer
  -> GameInputAction / GameLifecycleAction
```

Current baseline:

- `GameRoomData`
- `GameRoomPage`
- GameRoom-specific views

Target extension:

- `GameSessionDescriptor`
- `GameViewState`
- `IGameSurfaceRenderer`
- `IGameInputSink`

Allowed content:

- board, pieces, timers, move list, HUD, spectators, danmaku, room actions;
- optional low-frequency `FeatureSection` areas for static room metadata.

Not allowed:

- core board/HUD implemented as content `BlockView`;
- game rules in prefab scripts;
- app navigation directly controlled by game view nodes.

### FeaturePage

Used for profile, settings, data pages, match lobby, task center, and activity
pages.

Contract:

```text
FeaturePageController
  -> FeaturePageData
  -> FeaturePageRenderer or PrefabRenderer
  -> BlockActionData
```

Renderer:

- `FeaturePageRenderer` for data-described feature sections;
- future `PrefabRenderer` for designer-owned complex modules.

Allowed content:

- profile header, stats, rank list, settings list, action grids;
- complex prefab modules that still expose a binding contract.

Not allowed:

- feed/article body content;
- core game room runtime.

## Core Contracts

### Existing Contracts

The imported baseline already has these concrete contracts:

```text
PageData
  pageId
  pageType
  title
  blocks: List<BlockData>

BlockActionData
  type
  target
  parameters: Dictionary<string, string>

FeaturePageData
  pageId
  title
  sections: List<FeatureSectionData>

GameRoomData
  roomId
  title
  mode
  players
  pieces
  moveMarkers
  actions
  danmaku
```

These remain valid. New contracts should extend them rather than bypass them.

### Required Evolution Fields

Server-driven and config-driven payloads should add these fields as schemas
move beyond mock data:

```text
schemaVersion
rendererKey
prefabKey
resourceKeys
fallback
tracking
```

Field rules:

- `schemaVersion` is required once a payload can come from remote config.
- `rendererKey` chooses a renderer when `type` alone is too broad.
- `prefabKey` references a designer-owned visual module.
- `resourceKeys` points to images, videos, addressables, AB assets, or other
  runtime resources.
- `fallback` describes what to show when a type, prefab, or resource is missing.
- `tracking` provides analytics metadata; views must not invent analytics keys.

## Target Interfaces

These interfaces are the recommended next implementation boundary. They are not
all present in the baseline yet.

### IPage

```csharp
public interface IPage
{
    string PageId { get; }
    void OnEnter(PageEnterContext context);
    void OnExit(PageExitContext context);
    void HandleAction(BlockActionData action);
}
```

Purpose:

- gives `PageHost` one lifecycle shape for feed, article, feature, and game
  pages;
- keeps page lifecycle separate from concrete Unity view trees.

### IRenderer

```csharp
public interface IRenderer<in TData>
{
    void Render(TData data, Transform parent, Action<BlockActionData> onAction);
    void Clear();
}
```

Purpose:

- aligns `BlockRenderer`, `FeaturePageRenderer`, future prefab rendering, and
  future game surface rendering under the same lifecycle concept;
- keeps renderers responsible for display, not routing or services.

### IDataBoundView

```csharp
public interface IDataBoundView<in TData>
{
    void Bind(TData data, Action<BlockActionData> onAction);
}
```

Purpose:

- lets prefab-backed components participate in the same data/action pipeline;
- keeps prefabs as presentation and binding only.

### IGameSurfaceRenderer

```csharp
public interface IGameSurfaceRenderer
{
    void Render(GameViewState state, Transform parent, Action<GameInputAction> onInput);
    void SetLifecycle(GameSurfaceLifecycle lifecycle);
    void Release();
}
```

Purpose:

- isolates game rendering from news/article block rendering;
- gives game UI a state-driven contract without forcing it into `BlockData`.

## Renderer Registry

The current baseline already uses registries:

```text
BlockRegistry
  featured_match -> FeaturedMatchBlockView
  article_card -> NewsItemBlockView
  replay -> ReplayBlockView
  unknown -> UnknownBlockView

FeatureSectionRegistry
  quick_action_grid -> QuickActionGridSectionView
  profile_header -> ProfileHeaderSectionView
  unknown -> UnknownFeatureSectionView
```

The next step is not to replace these registries. It is to put a small routing
layer above them:

```text
RendererRegistry
  content.block -> BlockRegistry
  feature.section -> FeatureSectionRegistry
  prefab.module -> PrefabRendererRegistry
  game.surface -> GameSurfaceRegistry
```

Selection rule:

```text
PageController selects the domain renderer.
Domain renderer selects the concrete view/prefab by type, rendererKey, or prefabKey.
Views bind data and emit actions only.
```

Unknown type handling:

- unknown content block -> `UnknownBlockView` or ignored placeholder;
- unknown feature section -> `UnknownFeatureSectionView`;
- missing prefab -> fallback view with recoverable log;
- unknown game surface -> fail closed to a safe room error state, not a crash.

## Action Routing

All visual domains emit actions into one dispatcher shape.

```text
View / Prefab / GameSurface
  -> BlockActionData or GameInputAction
  -> PageController
  -> ActionDispatcher
  -> Router / Service / Analytics
```

Recommended route table:

```text
open_article
  target: article_detail
  owner: Router
  required params: articleId

open_game
  target: game_room
  owner: Router + GameService
  required params: gameId or roomId

open_feature
  target: profile/settings/match
  owner: Router
  required params: pageId

refresh_page
  target: current
  owner: PageController

share
  target: article/game/result
  owner: PlatformService

analytics_event
  target: event key
  owner: AnalyticsService

game_input
  target: game action id
  owner: GameHostController
```

Routing rules:

- a `BlockView` may emit `open_article`; it may not instantiate
  `ArticlePage`;
- a prefab may emit `open_game`; it may not load the game scene directly;
- a game surface may emit `exit_game`; it may not directly rebuild the feed;
- page controllers may translate local actions into app actions, but they should
  not duplicate global route logic.

## Minimal Closed Loop

The first implementation slice should validate this route:

```text
App launch
  -> AppShell creates Router/PageHost
  -> Router opens FeedPage
  -> FeedPageController loads first feed page
  -> BlockRenderer renders news card and game entry card
  -> user taps news card
  -> ActionDispatcher routes open_article
  -> ArticlePageController loads PageData
  -> ArticlePage renders body blocks and game entry
  -> user taps game entry
  -> ActionDispatcher routes open_game
  -> GameHostController creates GameSurface
  -> GameSurface renders GameViewState/GameRoomData
  -> user exits game
  -> ActionDispatcher routes back
  -> Router returns to ArticlePage or FeedPage
```

This slice proves the important boundaries:

- feed and article use content blocks;
- game entry is a block, but game runtime is not;
- all navigation is action-driven;
- renderer selection is registry-driven;
- the app can return through the same navigation stack.

## Extension Rules

Before adding a new UI type, answer these questions:

1. Is it content in a scrollable feed/article body?
   Use `BlockData` and `BlockRegistry`.

2. Is it a feature module or app function page?
   Use `FeaturePageData` and `FeatureSectionRegistry`, or a prefab with
   `IDataBoundView`.

3. Is it core game runtime UI?
   Use `GameViewState`/`GameRoomData` and `GameSurfaceRenderer`.

4. Is it a designer-owned complex visual module?
   Use prefab rendering, but bind through data and emit actions through the
   dispatcher.

5. Does it need a new route or action?
   Add it to the action route table and define required parameters.

Required deliverables for new UI surface:

- schema or data contract update;
- registry entry or prefab key;
- fallback behavior;
- mock data;
- action definition;
- validation note in the task thread.

## Dependency Rules

Allowed dependency direction:

```text
AppShell -> Router -> PageController -> Renderer -> View -> Unity UI
PageController -> Services
Renderer -> ResourceResolver
GameHostController -> GameLogic/Simulation
GameSurfaceRenderer -> GameViewState
```

Forbidden dependency direction:

```text
View -> Router
View -> Service
Prefab -> Service
BlockRenderer -> GameSimulation
GameSurface -> FeedPage/ArticlePage
Service -> Unity UI
```

Keeping these boundaries intact is more important than making every renderer
look identical. The product should feel like one app because lifecycle, action,
navigation, analytics, resources, and fallback behavior are unified.
