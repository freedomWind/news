# GameRoom Schema

## 目标

`GameRoomData` 用于描述观战、下棋和回放房间的 UI 拼装输入。它属于局内运行态页面协议，不属于 `BlockSchema` 或 `FeaturePageData`。

首版只落 UI 骨架和静态 mock 数据，不实现棋规、AI、帧同步、网络同步、计时扣减、聊天发送和胜负判定。

## 渲染入口

当前入口：

- 首页 `open_match + target = match_live`：打开观战房间。
- 对局页 `open_ai_practice`：打开人机陪练房间。
- 对局页 `start_ranked_match`：暂时打开下棋房间占位。

页面层通过 `GameRoomPage.Build(parent, data, onBack)` 打开覆盖层。`GameRoomPage` 内部使用 GameRoom 专用 View，不使用 `BlockRenderer` 或 `FeaturePageRenderer` 承载核心棋盘。

## GameRoomData

```text
GameRoomData
  roomId
  title
  mode
  roundText
  statusText
  countdownText
  viewerCount
  redPlayer
  blackPlayer
  pieces
  redCapturedPieces
  blackCapturedPieces
  moveMarkers
  actions
  danmaku
```

字段说明：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `roomId` | string | 房间或对局 id |
| `title` | string | 顶部标题 |
| `mode` | enum | `Player`、`AiTraining`、`Spectator` |
| `roundText` | string | 回合展示文本 |
| `statusText` | string | 当前局面状态文本 |
| `countdownText` | string | 当前倒计时展示文本 |
| `viewerCount` | int | 观战人数 |
| `redPlayer` / `blackPlayer` | object | 双方棋手摘要 |
| `pieces` | array | 棋盘棋子位置 |
| `redCapturedPieces` / `blackCapturedPieces` | array | 被吃棋子展示 |
| `moveMarkers` | array | 上一步或重点位置标记 |
| `actions` | array | 页面底部按钮或观战操作 |
| `danmaku` | array | 观战弹幕展示数据 |

## 子结构

`GamePlayerData`：

```text
playerId
displayName
rank
avatarText
timerText
sideText
side
active
localPlayer
```

`GamePieceData`：

```text
pieceId
text
side
x
y
highlighted
```

坐标约定：

- `x` 为 0-8。
- `y` 为 0-9。
- UI 坐标只用于首版展示，不代表最终规则引擎内部坐标。

`GameRoomActionData`：

```text
actionId
label
icon
```

首版 action 只输出日志或关闭页面，后续由 GameRoom controller 接管。

## 模式差异

### AiTraining / Player

当前 UI：

- 顶部返回和标题。
- 对手信息栏。
- 棋盘。
- 本方信息栏。
- 状态提示。
- 求和、认输、悔棋、复盘按钮。
- 退出确认弹窗。

边界：

- 点击棋子只记录 UI action。
- 不执行合法走法、AI 走子、计时扣减或胜负判断。

### Spectator

当前 UI：

- 顶部返回和观战人数。
- 双方对阵信息栏。
- 棋盘。
- 吃子展示。
- 上一步标记。
- 静态弹幕层。
- 评论输入占位、发送、献花和观战操作。

边界：

- 弹幕首版是静态展示。
- 评论和献花不写入服务端。
- 不接真实直播同步。

## 组件边界

当前 GameRoom UI 文件：

- `GameRoomPage`
- `ChessBoardView`
- `GameRoomPlayerViews`
- `GameRoomActionBarView`
- `GameRoomOverlayViews`
- `GameRoomStyle`
- `GameRoomUi`

规则：

- 棋盘、棋子、弹幕、计时、局内操作都留在 GameRoom 专用 View。
- `BlockView` 不承载 GameRoom 核心区域。
- `FeatureSectionView` 只可用于低频附属信息，不参与棋盘和运行态控制。
- 后续真实逻辑应通过 `MatchRoomController`、`IGameSimulation`、`IFrameSyncSession` 等运行态接口接入。

## 后续扩展

下一步建议：

- 补 `MatchRoomController`，把 UI action、权限、计时、同步状态和 simulation 隔离出来。
- 补 `PlayerMiniProfilePopover`，用于房间内点击头像查看轻量资料。
- 补真实棋盘 renderer 或 prefab 子组件，替换当前 uGUI 线条棋盘。
- 补 Unity 截图级验证，检查 375x812 和常见安卓比例。
