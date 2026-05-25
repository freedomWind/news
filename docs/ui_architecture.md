# UI 架构

> 运行时 UI 的总边界契约见 `ui_runtime_boundaries.md`。本文继续记录当前
> Unity UI 分层、已有组件和实现细节。

## 技术选型

运行时主 UI 使用：

- `uGUI`
- `TextMeshPro`

暂不使用 `UI Toolkit` 作为主运行时框架。原因是本项目包含棋盘预览、对局互动、移动端页面、图片内容和游戏逻辑接入，`uGUI` 与 Unity 运行时对象、Prefab、Canvas、事件系统的结合更成熟。

`UI Toolkit` 可在后续用于编辑器工具或内部配置工具，但不作为首版 App UI 主框架。

## 分层结构

整体分为多层：

```text
Page Layer
  HomePage / ArticlePage / MatchPage

Content Render Layer
  BlockRenderer / BlockRegistry / BlockView

Feed Service Layer
  IFeedPager / IFeedPageRemoteRepository / FeedPageRequest / FeedPageResult

Content Service Layer
  IContentService / IContentStore / ContentRequest / ContentResult

Replay Runtime Layer
  IReplayRuntime / ReplayRuntimeFactory / RenderTexture

Game Simulation Layer
  IGameSimulation / GameCommandData / GameSnapshotData / GameTickInput

Frame Sync Layer
  IFrameSyncSession / FrameCommandBatch / ConfirmedTick

Base Component Layer
  AppText / AppImage / AppButton / AppCard / AppSafeArea / AppScrollView

Unity UI Layer
  Canvas / RectTransform / Image / TextMeshProUGUI / Button / ScrollRect
```

## 基础组件层

基础组件层不重写 Unity UI，只做统一封装。

### AppText

职责：

- 统一使用 `TextMeshProUGUI`。
- 管理字体、字号、颜色、行距、溢出策略。
- 后续支持主题字体和 fallback 字体。

### AppImage

职责：

- 统一本地图片和远程图片加载入口。
- 支持占位图、失败图、加载状态。
- 后续支持图片缓存和圆角裁剪。

### AppButton

职责：

- 统一点击反馈。
- 统一禁用状态。
- 统一节流策略，避免重复点击。

### AppCard

职责：

- 统一卡片背景、圆角、阴影或描边。
- 用于首页重点卡片、资讯卡片、对局卡片。

### AppSafeArea

职责：

- 适配移动端刘海屏、状态栏、底部手势区域。
- 管理顶部栏和底部栏的安全距离。

### AppScrollView

职责：

- 封装 `ScrollRect`。
- 统一滚动区域、底部内边距、刷新入口。
- 后续扩展虚拟列表。

## 内容渲染层

内容渲染层是项目的核心。

### BlockRenderer

职责：

- 接收页面内容数据。
- 遍历 `blocks`。
- 根据 `block.type` 创建对应 `BlockView`。
- 将数据绑定到视图。
- 控制 block 间距和布局刷新。

### BlockRegistry

职责：

- 维护 `block.type` 到 View Prefab 或 View 类的映射。
- 提供统一注册入口。
- 避免页面层直接写大量 `switch`。

### BlockView

职责：

- 每种 block 对应一个 view。
- 只负责展示和局部交互。
- 不直接请求页面级数据。

示例：

```text
FeaturedMatchBlockView
BoardPreviewBlockView
SectionTitleBlockView
NewsItemBlockView
DividerBlockView
SpacerBlockView
```

## Feed 服务层

Feed 服务层负责首页、频道页、搜索结果等列表流，不通过 `IContentService` 拉完整首页。

职责：

- `IFeedPager`：管理 cursor、分页加载、预取和去重。
- `IFeedPageRemoteRepository`：远程分页接口抽象。
- `FeedPageRequest`：描述 feed 分页请求。
- `FeedPageResult`：返回一段 blocks、`nextCursor`、`hasMore` 和版本信息。

首版实现：

- `FeedPager`
- `MockFeedPageRemoteRepository`
- `HttpFeedPageRemoteRepository`

推荐首页流程：

```text
HomePage
  -> IFeedPager.Start()
  -> IFeedPageRemoteRepository.FetchPage(feedId, cursor, limit)
  -> BlockRenderer.Render(firstPage)
  -> 滚动接近底部时 BlockRenderer.Append(nextPage)
```

约束：

- 首页信息流不走 `IContentService.LoadPage`。
- Feed 分页使用 cursor，不依赖固定总长度。
- 页面非激活时暂停预取，恢复激活后继续确保 ahead buffer。

## 内容服务层

内容服务层负责详情页和普通静态页的数据获取和缓存，不使用 ECS。

职责：

- `IContentService`：加载或刷新文章、回放、普通页面内容。
- `IContentStore`：缓存 `PageData`。
- `ContentRequest`：描述页面请求。
- `ContentResult`：返回成功/失败和页面数据。
- `IContentLocalRepository`：本地数据库/磁盘缓存抽象。
- `IContentRemoteRepository`：远程接口抽象。
- `ContentCachePolicy`：缓存过期策略。

首版实现：

- `MemoryContentStore`
- `MockContentService`
- `CachedContentService`
- `MemoryContentLocalRepository`
- `MockContentRemoteRepository`

后续真实实现可扩展：

- `CachedContentService`
- `SQLiteContentLocalRepository`
- `HttpContentRemoteRepository`

约束：

- 内容服务可以使用异步网络、缓存和重试。
- 内容服务不进入 ECS World。
- UI 页面通过服务拿到 `PageData` 后交给 `BlockRenderer`。
- 首页信息流由 Feed 服务层负责。

推荐数据策略：

- 文章详情在用户点击后按需请求，并根据 `knownVersion` / TTL 判断是否需要更新。
- 回放数据按 `replayId/version` 独立缓存，播放或进入详情时再取完整 payload。
- 文章详情默认 TTL 为 24 小时。

## 页面层

页面层负责组合，不直接处理低层 UI 细节。

页面、渲染器和运行态组件的选择规则见 `ui_composition_rules.md`。后续新增页面、block、feature 或 GameRoom 组件时，必须先按该规则确认归属边界。

### HomePage

职责：

- 组织顶部栏、内容流、底部 Tab。
- 通过 `IFeedPager` 请求首页分页信息流。
- 将内容数据交给 `BlockRenderer`。
- 处理页面级跳转。

### TabPageHost

`TabPageHost` 是底部 tab 对应的页面容器。

职责：

- 为每个 tab 创建独立 `TabPageView`。
- 每个 `TabPageView` 持有自己的 `ScrollRect` 和 `ContentRoot`。
- 切换 tab 时只隐藏/显示页面，不销毁页面节点。
- 保留每个 tab 的滚动位置和已渲染内容。
- 提供横向切换动画。
- 通过 `OnEnter` / `OnExit` 挂接页面生命周期。
- 页面隐藏使用 `CanvasGroup` 控制透明度和交互，不禁用整个 hierarchy，减少切回大页面时的激活和布局重建成本。

当前生命周期策略：

- 首页 `OnEnter`：恢复 feed 预取；如 feed 数据有更新则补渲染。
- 首页 `OnExit`：暂停 feed 预取。
- 首页后台 feed 更新不会在切换帧同步重建；切回后延迟补渲染 pending blocks。
- 功能页：首轮直接渲染 mock 数据，切换时复用已创建节点。

### FeaturePageRenderer

功能型页面不使用 `BlockData/PageData`，避免把“对局”“我的”这类应用功能页塞进文章 block schema。

职责：

- 接收 `FeaturePageData`。
- 通过 `FeatureSectionRegistry` 按 section type 创建对应 section view。
- 运行时创建 uGUI 节点，不依赖传统 prefab。
- 复用 `BlockActionData` 做页面动作分发，保持导航入口统一。

当前用于：

- `DataPage` 占位页。
- `MatchPage` 对局入口、匹配、好友约战和最近对局。
- `ProfilePage` 用户信息、成就、段位、设置和底部信息。
- 后续完整 `PlayerProfilePage` 玩家资料页。

### 后续页面

后续页面应沿用同样模式：

- `ArticlePage`：文章 Header + BlockRenderer
- `MatchPage`：功能入口使用 FeaturePageRenderer；真实棋盘/对局运行域后续独立接入
- `DataPage`：榜单、统计、赛事数据使用 FeaturePageRenderer
- `ProfilePage`：用户信息和设置使用 FeaturePageRenderer

真实观战、对战和局内回放不使用 `BlockRenderer` 作为主结构。后续应通过 `GameRoomPage` 组合专用棋盘、计时、操作、弹幕和同步组件；低频附属信息可局部复用 Feature section。

当前首版 GameRoom UI 已有独立数据结构和 mock 数据，详见 `game_room_schema.md`。

玩家资料归类：

- 完整玩家资料页使用 `FeaturePageRenderer`。
- GameRoom 内双方玩家状态面板使用 GameRoom 专用 View。
- GameRoom 内点击头像弹出的轻量资料使用专用 popover，popover 内可跳转完整玩家资料页。

### ArticlePage Extensions

文章详情正文仍然由 `BlockRenderer` 渲染，互动和评论不进入 `BlockSchema`。

当前页面级扩展：

- 顶部收藏按钮：属于文章页导航栏状态。
- 正文结束标记：由 `ArticlePage` 在正文 blocks 之后追加。
- 互动区：献花、分享按钮，派发文章行为 action。
- 互动摘要：通过 `IArticleEngagementService` 一次加载 `ArticleEngagementSummaryData`。
- `ArticleEngagementSummaryData` 包含收藏状态、献花状态、献花数、评论数、评论预览和作者回复。
- 底部评论输入栏：固定在页面底部，不属于滚动正文。

这样正文排版、文章行为和评论系统三者保持独立，后续可以单独接真实评论分页、分享 SDK 和用户互动服务。

## 目录建议

```text
Assets/
  App/
    Runtime/
      UI/
        Base/
        Blocks/
        Features/
        Pages/
        Navigation/
        Theme/
      Data/
        Blocks/
        Features/
        GameRoom/
        Mock/
        Replay/
      Replay/
      Simulation/
        Commands/
        Snapshots/
        FrameSync/
      Services/
        Content/
        Feed/
        ImageLoading/
    Scenes/
      HomeDemo.unity
    Prefabs/
      UI/
        Base/
        Blocks/
        Pages/
```

## 依赖方向

依赖方向必须保持单向：

```text
Page -> Feed Service / Content Service -> Content Render -> Base Components -> Unity UI
Replay Runtime -> Game Simulation
Frame Sync -> Game Simulation
```

禁止：

- 基础组件反向依赖具体页面。
- BlockView 直接控制页面路由，应该通过事件通知页面层。
- 页面层直接操作某个 block 内部的具体子节点。

## 回放运行层

文章内游戏回放应通过独立运行域承载，不直接依赖主游戏页面。

首版接口：

```text
ReplayPreviewBlockView
  -> feed/list page
  -> no runtime
  -> open_article/open_replay action

ReplayBlockView
  -> article/detail page
  -> IReplayRuntime
  -> ReplayRuntimeFactory
  -> RenderTexture
  -> RawImage
```

当前只定义数据和接口，不实现帧同步和真实游戏逻辑。

职责边界：

- `ReplayPreviewBlockView` 只展示轻量摘要和入口，不创建 runtime。
- feed/list 页面只能使用 `replay_preview`，不能直接使用完整 `replay`。
- `ReplayBlockView` 用于详情页，负责绑定 runtime 输出纹理和控制条。
- `ReplayData` 描述初始局面、步骤列表、播放区间和渲染配置。
- `IReplayRuntime` 负责加载、播放、暂停、停止、跳步、逐帧推进和释放。
- `IReplayRuntime` 可绑定 `IGameSimulation`。
- `ReplayRuntimeFactory` 根据 `renderProfile.mode` 创建具体 runtime。
- `NullReplayRuntime` 是首版占位实现，只维护状态和 RenderTexture 生命周期。

后续真实实现可以扩展：

- `Canvas2DReplayRuntime`
- `Scene2DReplayRuntime`
- `Scene3DReplayRuntime`
- `AdditiveSceneReplayRuntime`

## 游戏模拟层

游戏模拟层是后续普通 C# 实现或 Unity Entities 实现的共同接口。

首版接口：

```text
IGameSimulation
  Load(GameSimulationConfig)
  EnqueueCommand(GameCommandData)
  Tick(GameTickInput)
  ApplySnapshot(GameSnapshotData)
  CreateSnapshot()
  Reset()
```

职责：

- 管理确定性 tick。
- 消费游戏命令。
- 生成快照。
- 接收快照恢复状态。
- 向回放和帧同步提供统一模拟接口。

当前实现：

- `NullGameSimulation`：占位实现，不执行棋规。

后续实现：

- `ClassicChessSimulation`：普通 C# 象棋逻辑。
- `EntitiesChessSimulation`：基于 Unity Entities 的象棋逻辑。

约束：

- UI 不直接访问具体 simulation 实现。
- 回放和对局页面依赖 `IGameSimulation`。
- Entities 作为模拟层实现选项，不成为 App UI 和内容服务的基础架构。

## 帧同步层

帧同步层只定义 tick 命令边界，不实现网络和回滚。

首版接口：

```text
IFrameSyncSession
  Start(FrameSyncConfig)
  SubmitLocalCommand(GameCommandData)
  SubmitRemoteBatch(FrameCommandBatch)
  TryGetCommandsForTick(tick)
  ConfirmTick(tick)
```

当前实现：

- `NullFrameSyncSession`：本地命令批次占位实现。

后续扩展：

- 网络命令同步。
- 输入延迟。
- tick 确认。
- 快照校验。
- 回滚和重放。
- ECS World 驱动模拟。

## 首版实现策略

首版可以先使用 mock 数据和手工绑定，等结构稳定后再接 JSON 解析。

建议顺序：

1. 搭建基础 Canvas、顶部栏、底部栏。
2. 创建首页内容区域。
3. 实现 block 数据模型。
4. 实现 `BlockRenderer`。
5. 实现首批 block view。
6. 用 mock 数据生成首页。
7. 补充验收和进度记录。
