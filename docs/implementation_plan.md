# 实施计划

## 推进方式

每个阶段都必须产出可运行、可检查的结果。不要先做大而全框架，再回头补页面。

固定推进循环：

```text
读取当前文档
  ↓
选择下一项未完成任务
  ↓
实现最小可运行版本
  ↓
检查或运行
  ↓
更新 progress_log 和 acceptance_checklist
  ↓
继续下一项
```

## 阶段 0：文档基线

目标：建立后续开发的方向约束。

任务：

- [x] 创建 `product_goal.md`
- [x] 创建 `ui_architecture.md`
- [x] 创建 `block_schema.md`
- [x] 创建 `ui_composition_rules.md`
- [x] 创建 `implementation_plan.md`
- [x] 创建 `acceptance_checklist.md`
- [x] 创建 `progress_log.md`

验收：

- `docs/` 下存在核心文档。
- 文档包含首版目标、边界、架构、UI 组合规则、schema、计划、验收标准和进度。

## 阶段 1：Unity 工程骨架

目标：建立 Unity 项目结构和基础目录。

任务：

- [x] 确认当前目录是否已有 Unity 工程。
- [x] 如无工程，创建 Unity 工程或放置约定目录结构。
- [x] 创建 `Assets/App/Runtime` 目录。
- [x] 创建 `Assets/App/Scenes` 目录。
- [x] 创建 `Assets/App/Prefabs` 目录。
- [~] 创建首页 Demo 场景。

验收：

- Unity 能打开项目。
- 能看到首个 Demo 场景。
- 目录结构符合 `ui_architecture.md`。

## 阶段 2：基础 UI 框架

目标：搭建首页所需基础 UI 结构。

任务：

- [x] 创建主 Canvas。
- [x] 创建顶部栏。
- [x] 创建底部 TabBar。
- [x] 创建首页滚动区域。
- [x] 创建安全区适配组件。
- [x] 接入 TextMeshPro。
- [x] 定义基础主题颜色、字号、间距。

验收：

- 竖屏下顶部栏、内容区、底部栏布局正确。
- 滚动区域不被底部栏遮挡。
- 中文文本显示正常。

## 阶段 3：Block 数据模型

目标：建立内容块数据结构。

任务：

- [x] 创建 Page 数据模型。
- [x] 创建 Block 基础数据模型。
- [x] 创建 Action 数据模型。
- [x] 创建首页 mock 数据。
- [x] 支持首批 block 类型的数据结构。

验收：

- 首页内容可以用 mock 数据完整描述。
- mock 数据覆盖重点对局、栏目标题、资讯列表项、棋盘预览。

## 阶段 4：BlockRenderer

目标：让首页内容由数据驱动生成。

任务：

- [x] 创建 `BlockRenderer`。
- [x] 创建 `BlockRegistry`。
- [x] 创建 `BlockView` 基类或接口。
- [x] 实现 block 视图创建和绑定。
- [x] 处理未知 block 类型。
- [x] 支持 block 间距。

验收：

- 首页主体不直接写死具体 block。
- 新增 block view 后可通过注册接入。
- 未知 block 不会导致页面崩溃。

## 阶段 5：首页首批 Block

目标：完成接近设计图的首页内容。

任务：

- [x] 实现 `FeaturedMatchBlockView`。
- [x] 实现 `BoardPreviewBlockView`。
- [x] 实现 `SectionTitleBlockView`。
- [x] 实现 `NewsItemBlockView`。
- [x] 实现 `SpacerBlockView`。
- [x] 实现 `DividerBlockView`。

验收：

- 首页能展示重点对局卡片。
- 首页能展示赛事快讯栏目。
- 首页能展示资讯列表项。
- 棋盘区域有占位展示。
- 视觉结构接近 `设计图/首页.jpeg`。

## 阶段 6：页面交互

目标：打通基础点击行为。

任务：

- [x] 实现简单页面路由接口。
- [x] block action 可通知页面层。
- [x] 首页 Tab 可点击并切换选中状态。
- [x] 对局卡片点击可输出或进入占位详情页。
- [x] 资讯项点击可输出或进入占位详情页。

验收：

- 点击不会无响应。
- block view 不直接控制页面跳转。
- 页面层能接收到 action。

## 阶段 7：文章详情基础

目标：验证图文混排扩展能力。

任务：

- [x] 创建文章详情页。
- [x] 支持 `paragraph` block。
- [x] 支持 `image` block 的占位加载。
- [x] 支持 `board_preview` 嵌入文章。
- [x] 支持从首页资讯进入详情页。

验收：

- 文章详情页也使用 `BlockRenderer`。
- 图文和棋盘预览可以混排。

## 阶段 8：性能和工程化

目标：补齐长期维护所需能力。

任务：

- [ ] 图片异步加载。
- [ ] 图片缓存。
- [ ] 长列表复用或虚拟列表。
- [ ] 资源释放策略。
- [ ] UI 事件节流。
- [ ] 基础日志。

验收：

- 首页内容增加后滚动仍稳定。
- 图片加载失败有占位。
- 重复进入页面不会明显泄漏对象。

## 阶段 9：回放运行域基础

目标：为文章内游戏回放定义数据和运行时接口。

任务：

- [x] 创建 `ReplayData`。
- [x] 创建 `ReplayStepData`。
- [x] 创建 `ReplayRenderProfileData`。
- [x] 在 `BlockData` 中挂载可选 `replay` payload。
- [x] 创建 `IReplayRuntime`。
- [x] 创建 `ReplayRuntimeState`。
- [x] 创建 `ReplayRuntimeOptions`。
- [x] 创建 `ReplayRuntimeFactory`。
- [x] 创建 `NullReplayRuntime` 占位实现。
- [x] 创建 `ReplayMockData` 示例数据。
- [x] 创建 `ReplayBlockView`。
- [x] 创建 `ReplayPreviewBlockView`。
- [x] 将 `replay` block 注册到 `BlockRegistry`。
- [x] 将 `replay_preview` block 注册到 `BlockRegistry`。
- [x] 在 mock 文章中加入回放 block。
- [x] 在首页 mock feed 中加入轻量回放预览 block。

验收：

- 回放数据能描述初始局面、步骤区间、步骤列表和渲染配置。
- 文章系统后续可以通过 `IReplayRuntime` 获取 `RenderTexture`。
- 文章详情页可以通过 `ReplayBlockView` 展示回放 `RenderTexture` 和基础控制条。
- 首页 feed 使用 `ReplayPreviewBlockView` 作为入口，不创建 runtime。
- 首版不实现真实帧同步和游戏规则。

## 阶段 10：中间架构接口

目标：定义内容服务、游戏模拟和帧同步接口，为普通 C# 或 Unity Entities 实现留出稳定边界。

任务：

- [x] 创建 `IContentService`。
- [x] 创建 `IContentStore`。
- [x] 创建 `ContentRequest`。
- [x] 创建 `ContentResult`。
- [x] 创建 `MemoryContentStore`。
- [x] 创建 `MockContentService`。
- [x] 创建 `IGameSimulation`。
- [x] 创建 `GameSimulationConfig`。
- [x] 创建 `GameCommandData`。
- [x] 创建 `GameTickInput`。
- [x] 创建 `GameTickResult`。
- [x] 创建 `GameSnapshotData`。
- [x] 创建 `GameSimulationEvent`。
- [x] 创建 `NullGameSimulation`。
- [x] 创建 `GameSimulationFactory`。
- [x] 创建 `IFrameSyncSession`。
- [x] 创建 `FrameSyncConfig`。
- [x] 创建 `FrameCommandBatch`。
- [x] 创建 `FrameSyncState`。
- [x] 创建 `NullFrameSyncSession`。
- [x] 让 `IReplayRuntime` 支持绑定 `IGameSimulation`。

验收：

- 内容拉取接口不依赖 ECS。
- UI 页面可以通过内容服务获取 `PageData`。
- 回放 runtime 可以绑定模拟接口。
- 游戏模拟接口可由普通 C# 或 Entities 实现。
- 帧同步接口只定义命令/tick 边界，不实现网络。

## 阶段 11：详情内容缓存策略

目标：支持文章、回放和普通详情页按需请求，并使用内存/本地缓存兜底。

任务：

- [x] 创建 `ContentCacheEntry`。
- [x] 创建 `ContentCacheMetadata`。
- [x] 创建 `ContentCachePolicy`。
- [x] 创建 `ContentPageTypes`。
- [x] 创建 `IContentClock`。
- [x] 创建 `SystemContentClock`。
- [x] 创建 `IContentLocalRepository`。
- [x] 创建 `IContentRemoteRepository`。
- [x] 创建 `MemoryContentLocalRepository`。
- [x] 创建 `MockContentRemoteRepository`。
- [x] 创建 `CachedContentService`。
- [x] 创建 `ContentServiceFactory`。
- [x] 创建 `ContentKinds`。
- [x] 创建 `ContentRefreshMode`。
- [x] 扩展 `ContentRequest`，支持 `Page/Article/Replay` 工厂方法和 `knownVersion`。
- [x] 扩展 `IContentStore`，支持保存 `ContentCacheEntry`。
- [x] 将文章详情页接入 `IContentService`。
- [x] 移除 `IContentService` 的 home/feed 请求入口。

验收：

- `LoadPage` 可以优先返回内存/本地缓存。
- 本地缓存缺失时可以从远程仓库获取。
- article/replay 请求默认缓存优先，缺失、过期或版本不一致时才检查远程。
- `PageUpdated` 只在远程版本变化时通知页面刷新。
- 文章详情 TTL 默认为 24 小时。
- 文章详情只在用户点击后请求对应 article。
- 首页信息流只通过阶段 14 的 `IFeedPager` 请求。
- 当前 demo 默认仍为 mock，不接真实数据库。

## 阶段 12：HTTP 内容远端仓库

目标：让内容服务可以通过 HTTP 获取服务器下发的 JSON 内容，同时保持 UI 和缓存层不依赖 `UnityWebRequest`。

任务：

- [x] 创建 `IContentHttpClient`。
- [x] 创建 `UnityWebRequestContentHttpClient`。
- [x] 创建 `ContentHttpRequest` 和 `ContentHttpResponse`。
- [x] 创建 `ContentHttpConfig`，统一管理 baseUrl、路径模板、请求头和超时。
- [x] 创建 `IContentJsonSerializer`。
- [x] 创建 `NewtonsoftContentJsonSerializer`。
- [x] 创建服务端 DTO。
- [x] 创建 `ContentServerDtoMapper`，将服务端 DTO 转为 `ContentCacheEntry`。
- [x] 创建 `HttpContentRemoteRepository`。
- [x] 在 `ContentServiceFactory` 中提供 HTTP 版本工厂方法。
- [x] 编写 `docs/content_http_contract.md`。
- [x] 引入 `Newtonsoft.Json` 支持对象形式参数和未知字段忽略。
- [x] 将默认 feed/article 路径更新到 `Content.Api` 规划形态。
- [x] 兼容 `success/data` 和旧 `code/message/data` 响应。
- [x] 创建文章互动摘要 HTTP DTO、远端仓库和服务装配。
- [ ] 接入真实服务端地址。
- [ ] 接入认证、签名或 token 刷新。
- [x] 增加 HTTP 重试和错误码策略。

验收：

- UI 层不引用 `UnityWebRequest`。
- `CachedContentService` 不关心远端数据来自 mock 还是 HTTP。
- HTTP 响应能映射为 `ContentCacheEntry`。
- 服务端 `version` 和 `expiresInSeconds` 能进入缓存元数据。
- 当前无真实服务器时，demo 仍可使用 mock 仓库运行。

## 阶段 13：媒体资源加载层

目标：让内容服务只负责文章结构和媒体引用，图片/视频等资源通过独立媒体服务加载和缓存。

任务：

- [x] 创建 `MediaAssetData`。
- [x] 扩展 `BlockData`，支持 `media/posterUrl/streamUrl/durationSeconds`。
- [x] 创建 `IMediaAssetService`。
- [x] 创建 `IMediaAssetCache`。
- [x] 创建 `MemoryMediaAssetCache`。
- [x] 创建 `MediaAssetRequest`、`MediaAssetResult`、`VideoSourceResult`。
- [x] 创建 `UnityMediaAssetService`。
- [x] 创建 `MediaAssetServiceFactory`。
- [x] 创建 `BlockRenderContext`，让 block view 通过上下文访问媒体服务。
- [x] `ImageBlockView` 改为通过媒体服务异步加载图片。
- [x] 服务端 DTO 和 mapper 支持 `media` 字段。
- [x] 创建 `VideoBlockView`。
- [ ] 接入真实视频播放器或 HLS 播放方案。
- [ ] 实现磁盘媒体缓存。
- [ ] 实现图片解码尺寸控制和内存淘汰策略。

验收：

- 文章 JSON 不携带媒体二进制。
- 图片 block 能先显示占位，再异步显示纹理。
- block view 不直接调用 `UnityWebRequest`。
- 媒体缓存与内容缓存分离。
- 视频 block 已有封面和元信息占位，但播放器可后续实现。

## 阶段 14：首页 Feed 分页和预取

目标：避免首页信息流一次性拉取过多内容，改为 cursor 分段请求，并在首页激活时维持 2-3 屏左右的 ahead buffer。

任务：

- [x] 创建 `FeedPageRequest`。
- [x] 创建 `FeedPageResult`。
- [x] 创建 `FeedPagerConfig`。
- [x] 创建 `IFeedPageRemoteRepository`。
- [x] 创建 `IFeedPager`。
- [x] 创建 `FeedPager`。
- [x] 创建 `MockFeedPageRemoteRepository`。
- [x] `HomeMockData` 支持 `CreatePaged(offset, limit, out totalBlocks)`。
- [x] `BlockRenderer` 支持 append block。
- [x] `HomePage` 改为启动时加载 feed 第一页。
- [x] `HomePage` 根据滚动剩余距离触发下一页预取。
- [x] 首页 Tab 非激活时停止预取，恢复激活后继续确保缓存。
- [x] 创建 HTTP 版 feed 分页仓库。
- [x] 增加分页结果本地持久化缓存。
- [ ] 实现 UI 虚拟列表或节点回收。
- [ ] 增加分页请求取消和错误重试。

验收：

- 首页不再依赖一次性完整 `PageData`。
- 首次只渲染 feed 第一段内容。
- 滚动接近底部时自动请求下一段。
- 页面激活时按剩余内容高度维持预取 buffer。
- 分页结果按 block id 去重。
- 当前实现已具备 mock 和 HTTP 分页仓库，真实服务端仍待联调。

## 阶段 15：SQLite 本地持久化

目标：把详情内容缓存和首页 feed 分页缓存落到 SQLite，避免重启后丢失已拉取内容。

任务：

- [x] 将 `Samples/Sqlite/SQLite.cs` 移入运行时工具层。
- [x] 保留 `SQLite4Unity3d` 原命名空间，避免改动第三方封装主体。
- [x] 扩展 raw row 查询能力，用于手写 SQL 缓存表。
- [x] 创建 `ISqliteDatabase` 适配层，隔离业务代码和第三方 SQLite API。
- [x] 创建内容详情缓存表 schema。
- [x] 创建 feed 分页缓存表 schema。
- [x] 创建 SQLite 版详情本地仓库。
- [x] 创建 SQLite 版 feed 分页本地仓库。
- [x] 在 `ContentServiceFactory` 中提供 SQLite 版本工厂方法。
- [x] 在真实启动配置中切换 mock/http/sqlite 组合。
- [x] 增加 `SQLiteMock` 模式，用于无服务器验证 SQLite 页面链路。
- [x] 增加缓存清理策略。
- [x] 增加 SQLite 运行时 smoke test。

验收：

- SQLite 第三方封装位于 `Assets/App/Runtime/Services/Persistence/Sqlite/SQLite4Unity3d/`。
- 示例业务代码仍留在 `Assets/Samples/Sqlite/`，不进入正式服务层。
- 内容详情缓存和 feed 分页缓存使用独立表。
- 媒体二进制不写入 SQLite。
- `IContentService` 和 `IFeedPager` 上层调用方不直接依赖 SQLite API。
- Unity Editor 菜单可执行 SQLite smoke test。
- `HomeDemoBootstrap` 可通过配置选择 mock、HTTP、mock+SQLite 或 HTTP+SQLite。
- `SQLiteMock/SQLiteHttp` 启动后可执行轻量缓存清理。
- Unity Editor 菜单可查看 SQLite 缓存统计并执行清理。

## 阶段 16：功能页数据驱动 UI

目标：让 `对局`、`数据`、`我的` 这类应用功能页也可以由数据驱动生成，但不复用文章/资讯 `BlockSchema`。

任务：

- [x] 创建 `FeaturePageData`。
- [x] 创建 `FeatureSectionData`。
- [x] 创建 `FeatureItemData`。
- [x] 创建 `IFeatureSectionView`。
- [x] 创建 `FeatureSectionViewBase`。
- [x] 创建 `FeatureSectionRegistry`。
- [x] 创建 `FeaturePageRenderer`。
- [x] 实现对局页所需 section view。
- [x] 实现我的页所需 section view。
- [x] 提供 `MatchMockData`。
- [x] 提供 `ProfileMockData`。
- [x] 提供 `FeatureMockData.CreateDataPage()`。
- [x] `HomePage` 接入底部 tab 切换。
- [x] 首页 tab 继续使用 `IFeedPager + BlockRenderer`。
- [x] 功能 tab 使用 `FeaturePageRenderer`。
- [x] feed 回调在首页非激活时不覆盖当前功能页。
- [x] 增加 tab 切换动画和页面节点缓存策略。
- [ ] 增加功能页 UI 自动化截图验证。

验收：

- 功能页数据结构和 block schema 分离。
- `对局` 和 `我的` 可由 mock data 运行时创建。
- 底部 tab 能在首页、数据、对局、我的之间切换。
- tab 切换时每个页面保留独立滚动位置和已创建节点。
- 首页 feed 预取只在首页激活时运行。
- 当前实现不依赖 prefab，section view 运行时创建 uGUI 节点。

## 阶段 17：文章详情互动和评论 MVP

目标：补齐文章详情结尾互动、评论预览和固定评论输入栏，同时保持正文 block schema 不被评论/分享业务污染。

任务：

- [x] 创建 `ArticleCommentData`。
- [x] 创建 `ArticleCommentReplyData`。
- [x] 创建 `ArticleEngagementSummaryData`。
- [x] 创建 `IArticleEngagementService`。
- [x] 创建 `MockArticleEngagementService`。
- [x] `ArticlePage` 顶栏增加收藏按钮。
- [x] `ArticlePage` 正文后追加结束标记。
- [x] `ArticlePage` 正文后追加献花和分享互动区。
- [x] `ArticlePage` 正文后追加评论预览。
- [x] `ArticlePage` 增加底部固定评论输入栏。
- [x] `ArticlePage` 支持由运行时传入 mock 或 HTTP 文章互动服务。
- [x] HTTP 模式下文章互动摘要可从 `/api/articles/{articleId}/engagement-summary` 拉取。
- [ ] 接入真实评论列表分页。
- [ ] 接入真实评论发布流程。
- [ ] 接入真实收藏、分享和互动状态写入服务。

验收：

- 正文内容仍由 `BlockRenderer` 渲染。
- 评论和互动数据不进入 `BlockData`。
- 收藏、献花、评论预览和作者回复由互动摘要一次返回。
- 底部输入栏不遮挡滚动正文。
- 评论预览可显示作者回复。
- 互动按钮通过 `BlockActionData` 风格 action 派发给页面路由层。

## 阶段 18：GameRoom UI 骨架

目标：把 `设计图/下棋.html` 和 `设计图/观战.html` 落成共享的 GameRoom UI 框架，只做界面拼装，不接真实对局逻辑。

任务：

- [x] 创建 `GameRoomData`。
- [x] 创建 `GamePlayerData`。
- [x] 创建 `GamePieceData`。
- [x] 创建 `GameRoomMockData.CreatePlayerRoom()`。
- [x] 创建 `GameRoomMockData.CreateSpectatorRoom()`。
- [x] 创建 `GameRoomPage`。
- [x] 创建 GameRoom 专用棋盘、玩家栏、操作栏、弹幕/吃子展示组件。
- [x] 首页 `open_match + match_live` 打开观战房间。
- [x] 对局页 AI陪练/开始匹配打开下棋房间占位。
- [x] 编写 `docs/game_room_schema.md`。
- [ ] 接入真实 `MatchRoomController`。
- [ ] 接入真实棋规、AI、帧同步或直播同步。
- [ ] 增加 Unity 截图级 UI 验证。

验收：

- 观战和下棋共用 `GameRoomPage`，通过 `mode/data` 区分。
- GameRoom 核心不使用 `BlockRenderer`。
- GameRoom 核心不使用 `FeaturePageRenderer`。
- 当前只实现 UI 和静态 mock 行为。
- `dotnet build newsframework.sln --no-restore` 通过。

## 当前优先级

当前优先级：

1. 阶段 0：文档基线
2. 阶段 1：Unity 工程骨架
3. 阶段 2：基础 UI 框架
4. 阶段 3：Block 数据模型
5. 阶段 4：BlockRenderer
6. 阶段 8：性能和工程化
7. 阶段 9：回放运行域基础
8. 阶段 10：中间架构接口
9. 阶段 11：内容启动与缓存策略
10. 阶段 12：HTTP 内容远端仓库
11. 阶段 13：媒体资源加载层
12. 阶段 14：首页 Feed 分页和预取
13. 阶段 15：SQLite 本地持久化
14. 阶段 16：功能页数据驱动 UI
15. 阶段 17：文章详情互动和评论 MVP
16. 阶段 18：GameRoom UI 骨架

除非明确变更目标，否则后续应按此顺序推进。
