# UI 组合规则

## 目标

本规则用于约束运行时 UI 拼接方式，避免后续功能增多后出现 schema 混用、View 类型爆炸、页面职责失控。

项目继续采用运行时数据驱动组装为主，传统 prefab 只作为复杂子组件或美术资源承载方式，不作为页面组织的主模式。

## 三类渲染域

### BlockRenderer

用于内容流。

适用：

- 首页 feed 内容区。
- 文章详情正文。
- 频道流、搜索结果、推荐列表。
- 图片、视频、段落、资讯卡、文章卡、轻量棋谱预览。

不适用：

- 棋局房间核心界面。
- 选子、落子、计时、弹幕、实时同步。
- 用户资料页、设置页、功能入口页的主体结构。

规则：

- `BlockView` 只负责展示当前 block 和局部点击。
- `BlockView` 不直接请求页面级数据。
- `BlockView` 不直接控制页面路由，只派发 `BlockActionData`。
- 文章互动、评论、收藏、分享状态不进入正文 `BlockData`。

### FeaturePageRenderer

用于功能模块页。

适用：

- 数据页。
- 对局入口页。
- 我的页。
- 设置页。
- 静态或低频刷新的功能模块。
- GameRoomPage 内部的附属静态模块，例如棋手资料、赛事信息、推荐复盘。

不适用：

- 首页 feed 内容流。
- 文章正文。
- 棋盘、棋子、计时器、走子列表、弹幕等运行态组件。

规则：

- `FeatureSectionView` 负责功能模块展示。
- 功能页通过 `FeaturePageData` 和 section type 组装。
- 功能页可复用 `BlockActionData` 做统一 action 分发。
- 不要把功能页硬塞进 `BlockData/PageData`。

### GameRoomPage

用于局内运行态。

适用：

- 观战房间。
- 人机对战。
- 好友对战。
- 排位对战。
- 赛事直播棋局。
- 可交互回放房间。

不适用：

- 普通文章详情。
- 首页 feed。
- 纯静态功能页。

规则：

- `GameRoomPage` 是房间容器，不是普通内容页面。
- 核心棋盘和对局状态不使用 `BlockRenderer`。
- 观战、对战、回放优先共用一个 `GameRoomPage` 框架，通过 mode/config/data 区分。
- 不要为每一种房间复制一套完整 Page，除非页面范式完全不同。

推荐结构：

```text
GameRoomPage
  GameRoomHeaderView
  PlayerPanelView
  BoardView
  PieceLayer
  MoveHintLayer
  TimerView
  MoveListView
  ActionBarView
  DanmakuLayer
  ChatPanelView
  OptionalFeatureSectionRenderer
  MatchRoomController
```

推荐模式：

```text
Spectator
  allowMove = false
  showDanmaku = true
  showGift = true
  showReturnToLive = true

Player
  allowMove = true
  showTimer = true
  showResign = true
  showUndo = depends on rule

Replay
  allowMove = false
  showPlaybackControls = true
  showMoveList = true
```

## 玩家资料归类

玩家相关 UI 分为三类，禁止混用。

### 完整玩家资料页

归属：`FeaturePageRenderer`。

适用入口：

- 我的页。
- 排行榜。
- 对局记录。
- GameRoom 中点击“查看主页”。
- 关注/粉丝/搜索结果。

推荐页面：

```text
PlayerProfilePage
  -> FeaturePageRenderer
  -> FeaturePageData
```

推荐 sections：

```text
profile_header
stats_row
achievement_strip
recent_match_list
action_grid
about_footer
```

可展示：

- 头像、昵称、段位、地区。
- 胜率、总局数、连胜。
- 最近战绩。
- 常用开局。
- 成就徽章。
- 关注、私信、约战按钮。
- 最近发布内容或历史对局入口。

规则：

- 完整玩家资料页不使用 `BlockRenderer`。
- 完整玩家资料页不属于 GameRoom 核心运行态。
- 玩家资料数据可以先转成 `FeaturePageData` 再交给 renderer。

### GameRoom 内玩家状态面板

归属：GameRoom 专用 View。

推荐组件：

```text
PlayerPanelView
PlayerClockView
PlayerConnectionStateView
```

可展示：

- 双方头像、昵称、段位。
- 当前轮到谁。
- 剩余时间、读秒。
- 是否离线、托管、等待。
- 悔棋、求和、认输等局内状态。

规则：

- 不使用 `FeaturePageRenderer` 承载该区域。
- 不使用 `BlockView` 承载该区域。
- 由 `GameRoomPage` 和 `MatchRoomController` 根据局内状态驱动更新。

### GameRoom 内玩家资料浮层

归属：专用浮层 View。

推荐组件：

```text
PlayerMiniProfilePopover
```

适用：

- 房间内点击头像后快速查看。
- 不离开当前房间。
- 展示轻量资料和操作入口。

规则：

- 浮层可以复用 Feature section 的展示思路，但不通过完整 `FeaturePageRenderer` 承载。
- 浮层内的“查看主页”再打开完整 `PlayerProfilePage`。
- 浮层不得直接修改 GameRoom 核心状态，只派发 action。

## 页面职责

### Page

负责页面框架和生命周期。

可以做：

- 顶部栏、底部栏、返回层、tab 容器。
- 选择数据服务。
- 创建 renderer。
- 处理页面级 action。
- 打开/关闭子页面。

不应该做：

- 堆大量具体卡片布局细节。
- 直接操作某个 block 内部子节点。
- 承担复杂业务状态机。

### Renderer

负责按数据创建 View。

可以做：

- 清空、追加、复用节点。
- 按 type 创建 View。
- 绑定数据和 action 回调。

不应该做：

- 直接访问网络服务。
- 直接处理导航。
- 维护页面业务状态。

### View

负责展示和局部交互。

可以做：

- 创建自身 UI 层级。
- 绑定当前 data。
- 发出 action。
- 管理自身异步资源加载回调。

不应该做：

- 打开页面。
- 修改全局状态。
- 读取其他页面数据。

### Controller

负责运行态逻辑。

适用：

- 棋局同步。
- 走子处理。
- 回放控制。
- 断线重连。
- 用户输入权限。

不应该做：

- 直接拼 UI 层级。
- 直接依赖具体页面节点细节。

## Schema 规则

### 新增 block type 前

必须先判断：

- 是否只是已有 View 的文案、图标、间距或标签差异。
- 是否可以通过已有字段参数化。
- 是否有明确业务语义和交互差异。
- 是否需要服务端长期下发。

只有业务语义或交互差异明确时才新增 `BlockView`。

### 字段语义

禁止长期复用含义不清的字段。

临时复用可以接受，但必须尽快沉淀到 schema 文档：

```text
type
required fields
optional fields
action
rendering notes
fallback behavior
```

示例：

```text
live_match_item
  title: 对阵双方
  subtitle: 回合和围观数
  badge: 棋类或图标短文本
  action: open_match
```

如果某个字段在多个 type 中含义不同，必须在 schema 中逐 type 说明。

### Unknown type

客户端遇到未知 block/feature type 必须降级：

- 不崩溃。
- 展示占位或忽略。
- 输出可定位日志。

## Registry 规则

- `BlockRegistry` 只注册内容流 block。
- `FeatureSectionRegistry` 只注册功能模块 section。
- GameRoom 专用组件不注册到 `BlockRegistry`。
- 新增 registry 项必须同步 mock 数据和 schema 文档。
- Registry 变长时优先检查是否存在可参数化的重复 View。

## 复用规则

可以复用：

- `AppUIFactory`
- `AppTheme`
- `BlockActionData`
- `MediaAssetService`
- `FeatureSectionView` 的低频展示模块

谨慎复用：

- `BlockView` 到非内容页面。
- `FeatureSectionView` 到实时运行态区域。

禁止复用：

- 用 `BlockView` 承载棋盘、棋子、计时、弹幕、走子输入。
- 用 `FeaturePageRenderer` 承载完整游戏房间核心。

## Prefab 使用规则

运行时拼接是主模式，prefab 可用于：

- 复杂棋盘子组件。
- 美术资源复杂的固定组件。
- 动画或 Timeline 强依赖组件。
- 3D 场景或棋盘实体层。

prefab 不应该用于：

- 整个首页。
- 整个文章详情。
- 整个功能页。
- 可以由 schema 稳定描述的内容流卡片。

## GameRoom 边界

GameRoom 可以复用数据驱动思想，但不复用内容流模型作为核心。

推荐请求：

```text
GameRoomRequest
  roomId
  gameType
  mode
  layoutType
  dataSource
```

推荐数据：

```text
GameRoomData
  roomInfo
  players
  boardState
  moveList
  timers
  permissions
  staticSections
```

其中：

- `staticSections` 可交给 Feature section 渲染。
- `boardState/moveList/timers/permissions` 必须交给 GameRoom 专用 runtime view 和 controller。
- 首版 UI schema 和当前 mock 数据记录在 `game_room_schema.md`。

## 验收要求

每新增一种页面、block、feature 或 game room 组件，至少满足：

- 有 mock 数据。
- 有明确 action。
- 有 schema 或规则说明。
- 不破坏现有 mock demo。
- `dotnet build newsframework.sln --no-restore` 通过。

后续应补充：

- Unity 编辑器 smoke scene。
- 截图级 UI 验证。
- 长文本和窄屏布局检查。
