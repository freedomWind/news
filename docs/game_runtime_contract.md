# Game Runtime Contract

## Purpose

This document defines the minimum game runtime contract for the short-news +
lightweight game app. It is the implementation baseline for wrapping the
current Xiangqi demo behind `GameHost` / `GameSession`, and the QA baseline for
game entry, lifecycle, input, result, error, and fallback validation.

The contract keeps three boundaries separate:

- content and feature pages describe game entry points;
- `GameHost` owns app integration and lifecycle;
- game runtime owns state, input, rendering, resources, and results.

Related documents:

- `ui_runtime_boundaries.md`: app-level UI and renderer boundaries.
- `game_room_schema.md`: current GameRoom UI data and existing demo surface.
- `block_schema.md`: content block schema, including game/replay entry blocks.
- `ui_composition_rules.md`: rules that prevent GameRoom from becoming a
  content block.

## Current Baseline

Already present in the imported project:

```text
GameRoomData / GameRoomPage
  enough for the current demo room UI and static interactions

IReplayRuntime / ReplayRuntimeState
  replay runtime lifecycle skeleton

IGameSimulation
  Load / EnqueueCommand / Tick / ApplySnapshot / CreateSnapshot / Reset

GameCommandData / GameSnapshotData / GameTickInput / GameTickResult
  internal simulation command and snapshot primitives

ChessSimulation / GameSimulationFactory
  Xiangqi-specific local simulation skeleton
```

Missing layer:

```text
GameEntryData
GameLaunchRequest
GameSession state machine
GameViewState
GameInputAction
GameResultData
GameResourceManifest
GameHostController
```

The existing simulation types are lower-level game logic primitives. They should
not become the app entry contract.

## Runtime Layers

```text
Feed / Article / Feature Page
  -> GameEntryData
  -> BlockActionData { type=open_game/open_replay, params... }

AppShell / ActionDispatcher
  -> GameLaunchRequest
  -> GameHostController

GameHostController
  -> GameSession
  -> GameResourceManifest
  -> IGameRuntime
  -> GameViewState
  -> IGameSurfaceRenderer

GameSurfaceRenderer
  -> GameInputAction
  -> GameSession

GameSession
  -> GameResultData / lifecycle action
  -> AppShell / ActionDispatcher
```

Ownership:

| Layer | Owns | Does not own |
| --- | --- | --- |
| Feed/Article/Feature | game entry metadata and launch action | game loading, game state, game resources |
| `ActionDispatcher` | translating app action into launch request | game rule execution |
| `GameHostController` | lifecycle, mounting, return path, errors, observability | board rendering details, rule decisions |
| `GameSession` | state machine, runtime selection, input handling, result | app navigation policy |
| `IGameRuntime` | local/remote/replay state source | Unity UI hierarchy |
| `IGameSurfaceRenderer` | `GameViewState` to UI/prefab | game rules, service calls, app routes |

## GameEntryData

`GameEntryData` is the content-side description of a game entry. It can appear
inside feed blocks, article blocks, feature sections, or remote CMS data.

```text
GameEntryData
  entryId
  gameId
  title
  subtitle
  mode
  roomId
  replayId
  sourceContentId
  launchParams
  requiredAssets
  fallback
  tracking
```

Fields:

| Field | Required | Description |
| --- | --- | --- |
| `entryId` | yes | unique id for this visible entry |
| `gameId` | yes | game type, for example `xiangqi` |
| `title` | yes | user-facing entry title |
| `subtitle` | no | entry description |
| `mode` | yes | `spectator`, `ai_training`, `player`, `replay` |
| `roomId` | conditional | required for spectator/player room entry |
| `replayId` | conditional | required for replay entry |
| `sourceContentId` | no | article/feed/content id that produced the entry |
| `launchParams` | no | string key/value params forwarded to launch request |
| `requiredAssets` | no | resource keys used for preflight checks |
| `fallback` | yes | what to do when launch cannot continue |
| `tracking` | no | analytics metadata |

Rules:

- `GameEntryData` is not a game state.
- `GameEntryData` must be serializable by content services.
- `GameEntryData` may be embedded in `BlockData`, but the actual game runtime
  must not be a `BlockView`.

Example:

```json
{
  "entryId": "home_xiangqi_live_001",
  "gameId": "xiangqi",
  "title": "象棋直播间",
  "mode": "spectator",
  "roomId": "room_1001",
  "sourceContentId": "feed_home",
  "launchParams": {
    "side": "red",
    "layout": "portrait_compact"
  },
  "requiredAssets": ["game.xiangqi.board.default", "game.xiangqi.pieces.redblack"],
  "fallback": {
    "type": "open_article",
    "target": "article_detail",
    "message": "游戏暂时无法打开，已切换到图文复盘。"
  }
}
```

## GameLaunchRequest

`GameLaunchRequest` is created by `ActionDispatcher` or `GameHostController`
from a user action. It is the app-side command to enter a game.

```text
GameLaunchRequest
  launchId
  gameId
  mode
  roomId
  replayId
  sourcePageId
  sourceContentId
  returnRoute
  initialState
  initialStateFormat
  dataSource
  launchParams
  resourcePolicy
  tracking
```

Fields:

| Field | Required | Description |
| --- | --- | --- |
| `launchId` | yes | unique id for this launch attempt |
| `gameId` | yes | game type |
| `mode` | yes | `spectator`, `ai_training`, `player`, `replay` |
| `roomId` | conditional | room id for live/player entry |
| `replayId` | conditional | replay id for replay entry |
| `sourcePageId` | yes | page that emitted the action |
| `sourceContentId` | no | article/feed/content id |
| `returnRoute` | yes | where AppShell returns after exit/error |
| `initialState` | no | local game state, for example FEN |
| `initialStateFormat` | no | `fen`, `json`, `snapshot`, etc. |
| `dataSource` | yes | `mock`, `local_simulation`, `remote_room`, `replay` |
| `launchParams` | no | string key/value launch params |
| `resourcePolicy` | yes | preload/fallback/timeout requirements |
| `tracking` | no | analytics metadata |

Rules:

- every launch has a `returnRoute`;
- invalid launch requests fail before assets are loaded;
- launch request validation should produce a structured `GameErrorData`.

## GameSession State Machine

`GameSession` is the runtime lifecycle owner. It bridges the app request,
resources, runtime state source, renderer, and result.

```text
Empty
  -> Preparing
  -> LoadingAssets
  -> Ready
  -> Playing
  -> Paused
  -> Result
  -> Exiting
  -> Disposed

Any state except Disposed
  -> Error
  -> Exiting
  -> Disposed
```

State meanings:

| State | Meaning | Allowed next states |
| --- | --- | --- |
| `Empty` | session object exists but has no launch request | `Preparing`, `Disposed` |
| `Preparing` | validate request, resolve game descriptor and runtime mode | `LoadingAssets`, `Error`, `Exiting` |
| `LoadingAssets` | load required resources and fallback candidates | `Ready`, `Error`, `Exiting` |
| `Ready` | resources loaded, runtime created, first view state available | `Playing`, `Paused`, `Exiting` |
| `Playing` | accepts gameplay input and ticks/listens to runtime | `Paused`, `Result`, `Error`, `Exiting` |
| `Paused` | app paused, overlay shown, or page not active | `Playing`, `Exiting`, `Error` |
| `Result` | terminal game result is available | `Exiting`, `Playing` for rematch only |
| `Error` | recoverable or terminal failure is available | `Exiting`, `Preparing` for retry |
| `Exiting` | result and cleanup are being reported to AppShell | `Disposed` |
| `Disposed` | runtime, renderer, resources, and listeners are released | none |

Required events:

```text
session_created
state_changed
launch_validated
asset_load_started
asset_load_failed
first_view_state_ready
input_received
input_rejected
result_created
error_created
session_exiting
session_disposed
```

Observability rule:

- each event must include `launchId`, `sessionId`, `gameId`, `mode`, and
  `state`.

## GameViewState

`GameViewState` is the only data shape that the game UI/prefab should render.
It is not the full game logic state.

```text
GameViewState
  sessionId
  gameId
  mode
  phase
  tick
  version
  board
  players
  hud
  availableInputs
  permissions
  overlays
  result
  error
  resources
  trackingContext
```

Fields:

| Field | Description |
| --- | --- |
| `sessionId` | current runtime session |
| `gameId` | game type |
| `mode` | spectator / AI / player / replay |
| `phase` | maps to the current session-visible phase |
| `tick` | game/simulation tick or remote frame number |
| `version` | state schema version |
| `board` | board/grid/piece view state, game-specific payload |
| `players` | public player state shown by UI |
| `hud` | timers, labels, status text, score, round |
| `availableInputs` | input actions the UI may expose |
| `permissions` | local player capabilities |
| `overlays` | modal, loading, result, error, reconnect |
| `result` | optional terminal result |
| `error` | optional current error |
| `resources` | resolved resource keys for renderer |
| `trackingContext` | analytics context |

Rules:

- renderer reads `GameViewState`; it does not read `IGameSimulation` directly;
- state may be converted from `GameRoomData` during the first migration;
- unknown optional fields must degrade without crash;
- `availableInputs` is the source of truth for enabled/disabled UI commands.

First migration mapping:

```text
GameRoomData.roomId        -> GameViewState.sessionId or board.roomId
GameRoomData.mode          -> GameViewState.mode
GameRoomData.pieces        -> GameViewState.board.pieces
GameRoomData.players       -> GameViewState.players
GameRoomData.actions       -> GameViewState.availableInputs
GameRoomData.statusText    -> GameViewState.hud.statusText
GameRoomData.countdownText -> GameViewState.hud.timerText
GameRoomData.danmaku       -> GameViewState.overlays.danmaku
```

## GameInputAction

`GameInputAction` is emitted by `IGameSurfaceRenderer` or prefab-bound game
views. It is the only input shape sent into `GameSession`.

```text
GameInputAction
  actionId
  type
  source
  playerId
  sequence
  tick
  target
  payload
  parameters
  clientTimeMs
```

Recommended types:

```text
move_piece
select_piece
use_item
pause
resume
exit
resign
request_draw
undo
restart
send_comment
flower
share
open_profile
replay_play
replay_pause
replay_seek
```

Rules:

- renderer may only emit actions listed in `GameViewState.availableInputs`;
- `GameSession` validates every input before passing it to simulation, remote
  room, or replay runtime;
- invalid input returns `GameInputResult` with `accepted=false` and a structured
  reason;
- app navigation actions such as `share` and `open_profile` are bridged back to
  `ActionDispatcher`, not executed by the game view.

Mapping to existing simulation:

```text
GameInputAction(type=move_piece)
  -> GameCommandData(type=move, payload/parameters...)
  -> IGameSimulation.Tick(...)
  -> GameTickResult
  -> GameViewState
```

## GameResultData

`GameResultData` is emitted when a session reaches `Result` or `Exiting`.

```text
GameResultData
  sessionId
  gameId
  mode
  outcome
  reason
  winnerPlayerId
  score
  durationSeconds
  finalTick
  snapshot
  sharePayload
  returnAction
  analytics
```

Outcome values:

```text
win
lose
draw
abandoned
exited
replay_finished
error
```

Rules:

- every normal exit emits a result, even if the user simply leaves;
- abnormal exits include `outcome=error` and `reason`;
- `returnAction` is interpreted by `ActionDispatcher`;
- result payload must not force direct navigation from the renderer.

## GameErrorData

```text
GameErrorData
  code
  severity
  message
  recoverable
  retryAction
  fallbackAction
  source
  details
```

Recommended codes:

```text
invalid_launch_request
unknown_game_id
unsupported_mode
missing_required_asset
asset_load_timeout
runtime_create_failed
state_decode_failed
illegal_input
network_disconnected
remote_room_closed
simulation_error
renderer_error
```

Severity:

```text
info
warning
recoverable
fatal
```

Rules:

- error must be visible to QA through logs or debug output;
- user-facing message may be simplified, but logs must include `code`;
- fatal errors must lead to `Exiting` and return control to AppShell;
- recoverable errors may expose retry or fallback action.

## GameResourceManifest

`GameResourceManifest` describes game runtime assets and fallback.

```text
GameResourceManifest
  gameId
  version
  mode
  prefabKeys
  addressableKeys
  assetBundleKeys
  mediaKeys
  audioKeys
  shaderKeys
  timeoutMs
  fallback
```

Rules:

- `GameHost` resolves the manifest before `Ready`;
- required assets must be marked as required or optional;
- missing optional assets degrade locally;
- missing required assets produce `missing_required_asset`;
- manifest resolution logs the resolved keys and selected fallback.

Example:

```json
{
  "gameId": "xiangqi",
  "version": "1",
  "mode": "ai_training",
  "prefabKeys": [
    { "key": "Prefabs/GameSurface/GameRoomSurface", "required": false },
    { "key": "Games/Xiangqi/ResultPanel", "required": false }
  ],
  "assetBundleKeys": [
    { "key": "game_xiangqi_board_default", "required": true },
    { "key": "game_xiangqi_effects_basic", "required": false }
  ],
  "timeoutMs": 8000,
  "fallback": {
    "type": "minimal_surface",
    "message": "使用基础棋盘模式。"
  }
}
```

## Runtime Modes

### Spectator

```text
GameLaunchRequest(mode=spectator, roomId)
  -> remote_room or mock room data
  -> GameViewState
  -> readonly GameSurface
```

Required behavior:

- move inputs are not available;
- comment/flower/share may be available if exposed by `availableInputs`;
- remote disconnection enters recoverable error or readonly stale state.

### AI Training

```text
GameLaunchRequest(mode=ai_training, gameId, initialState?)
  -> local_simulation
  -> GameViewState
  -> local input
```

Required behavior:

- local move input is validated;
- illegal move returns `illegal_input`;
- result is generated from simulation or explicit exit.

### Replay

```text
GameLaunchRequest(mode=replay, replayId)
  -> IReplayRuntime
  -> GameViewState
  -> replay controls
```

Required behavior:

- supports play/pause/seek inputs;
- no gameplay mutation input;
- replay runtime errors are translated into `GameErrorData`.

## GameHostController Responsibilities

`GameHostController` is the app integration point for task #8.

Must do:

- validate `GameLaunchRequest`;
- create and own one active `GameSession` per host;
- resolve `GameResourceManifest`;
- mount/unmount `IGameSurfaceRenderer`;
- forward app pause/resume to session;
- route game lifecycle actions back to `ActionDispatcher`;
- emit observable logs for state changes, input decisions, result, and error.

Must not do:

- implement Xiangqi rules;
- directly mutate board UI children;
- read feed/article data after launch;
- bypass `ActionDispatcher` for app navigation.

## Minimal Interfaces

These are target shapes for the next implementation task. They can be adjusted
to fit Unity conventions during implementation.

```csharp
public interface IGameSession
{
    string SessionId { get; }
    GameSessionState State { get; }
    GameLaunchRequest LaunchRequest { get; }
    GameViewState CurrentViewState { get; }

    void Prepare(GameLaunchRequest request);
    void Start();
    void Pause();
    void Resume();
    GameInputResult HandleInput(GameInputAction action);
    GameResultData Exit(string reason);
    void Dispose();
}
```

```csharp
public interface IGameRuntime
{
    string RuntimeId { get; }
    GameViewState CreateInitialState(GameLaunchRequest request);
    GameViewState Tick(float deltaTime);
    GameInputResult HandleInput(GameInputAction action);
    GameResultData CreateResult(string reason);
}
```

```csharp
public interface IGameSurfaceRenderer
{
    void Mount(Transform parent, GameResourceManifest resources);
    void Render(GameViewState state, Action<GameInputAction> onInput);
    void SetSessionState(GameSessionState state);
    void Unmount();
}
```

## Implementation Path For Task #8

Task #8 should not rewrite the whole game UI. It should wrap the existing demo:

1. Add contract classes/enums under a game runtime namespace.
2. Add `GameHostController` that accepts `GameLaunchRequest`.
3. Add a simple `GameSession` state machine with logs.
4. Convert existing `GameRoomData` into initial `GameViewState`, or keep it as
   a compatibility payload inside `GameViewState`.
5. Add `GameRoomSurfaceRenderer` that loads a game surface prefab root and
   mounts existing `GameRoomPage` through `GameRoomSurfaceView`.
6. Map existing `GameRoomActionData` into `GameInputAction`.
7. Route exit/result back to AppShell action handling.
8. Keep `IGameSimulation` integration minimal: AI training can start with mock
   or current `ChessSimulation`, but all input must pass through `GameSession`.

## Validation Contract

Documentation validation for task #7:

- state machine is closed and lists allowed transitions;
- `GameEntryData`, `GameLaunchRequest`, `GameViewState`, `GameInputAction`,
  `GameResultData`, `GameErrorData`, and `GameResourceManifest` have fields and
  rules;
- fallback/error rules are explicit;
- spectator, AI training, and replay paths are covered;
- QA-observable events are listed.

Implementation validation for task #8:

```text
enter
  -> Preparing
  -> LoadingAssets
  -> Ready
  -> Playing
  -> input accepted/rejected
  -> Result or Exiting
  -> return to Article/Feed
```

Required edge cases:

- repeated enter/exit does not leak active sessions;
- missing optional resource uses fallback;
- missing required resource returns `missing_required_asset`;
- illegal input returns `accepted=false` with reason;
- app pause/resume updates session state;
- runtime error returns control to AppShell;
- replay input cannot mutate gameplay state.

Minimum QA logs:

```text
launchId
sessionId
gameId
mode
state transition
resource manifest result
input action and accepted/rejected result
result outcome
error code and fallback action
return route
```
