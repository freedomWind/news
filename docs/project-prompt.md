# NewsFramework 项目重构提示词

> 使用此文档作为 AI 辅助开发的输入，可从零重建整个项目代码（不含 Unity 插件配置、ProjectSettings 等）。

---

## 一、项目概述

本项目是一个**象棋新闻资讯类 App**，基于 **Unity + C# + uGUI + TextMeshPro** 构建。所有 UI 界面均通过代码程序化生成，不使用 Prefab。

**目标平台**：移动端竖屏，参考分辨率 375×812。

**设计风格**：暖色纸质底色 (#F6F5F1)，中国红强调色 (#9A3430/#B5433A)，深木色顶部/底部栏 (#2F2517)，金色点缀 (#D4A574)。

---

## 二、项目目录结构

```
Assets/App/
  Core/              基础工具类
  Editor/            编辑器工具（场景构建、UI预览、诊断菜单）
  Resources/
    Font/            思源黑体 Normal + Bold（OTF + SDF）
    Textures/        棋盘纹理
  Runtime/
    AppBootstrap.cs        启动入口
    HomeDemoBootstrap.cs   场景引导
    Data/            数据模型 + Mock数据
      Blocks/        BlockData, PageData, BlockActionData
      Features/      FeaturePageData, FeatureSectionData, FeatureItemData
      Articles/      文章评论、互动数据
      GameRoom/      对局室数据
      Media/         媒体资源数据
      Replay/        棋谱回放数据
      Mock/          全部Mock数据
    UI/
      Base/          AppUIFactory, ChessBoardRenderer, ModalOverlay, SafeAreaFitter
      Theme/         AppTheme（颜色、尺寸、字体）
      Blocks/        15种 Block 组件 + 注册表 + 渲染器
      Features/      10种 FeatureSection 组件 + 注册表 + 渲染器
      GameRoom/      棋盘视图、对局室页面、玩家面板、弹窗
      Pages/         HomePage, ArticlePage, TabPageHost/TabPageView
    Services/
      Content/       内容服务（缓存、HTTP、Mock、分页）
      Media/         媒体服务（图片加载、视频源解析）
      Article/       文章互动服务
      Persistence/   SQLite持久化
    Simulation/
      Chess/         中国象棋引擎（棋盘、规则、AI、棋谱记录）
      FrameSync/     帧同步抽象层
    Replay/          棋谱回放运行时
  Scenes/            场景文件
```

**程序集划分**（asms 定义）：Core, Data, Editor, Replay, Runtime, Services, Simulation, UI。

---

## 三、Core 基础工具

命名空间 `NewsFramework.Core`。

### 3.1 ObjectPool\<T\>

线程安全通用对象池，使用 `ConcurrentQueue<T>`。提供 `Get()`、`Return(T)`、`Clear()`、`GetStatus()`（返回 `ObjectPoolStatus`：PooledCount、ActiveCount、TotalCount、MaxSize）。

构造函数参数：factory（`Func<T>`）、resetAction（`Action<T>`，对象归还时调用）、maxSize。

### 3.2 IFrameBuffer\<T\> / CircularFrameBuffer\<T\>

帧数据缓冲区接口和实现。基于 `ConcurrentDictionary<int, T[]>` 存储按帧号索引的数据数组。

- `AddFrameData(int, T[])` — 添加帧数据
- `TryGetFrameData(int, out T[])` — 获取并移除
- `CleanupExpiredData(int currentTick, int maxAge)` — 清理过期帧
- `GetStatus()` — 返回 `FrameBufferStatus`（TotalFrames、PendingFrames、ProcessedFrames、DroppedFrames）

### 3.3 EventBus

类型化事件总线，单例模式。

- `Subscribe<T>(Action<T>)` / `Unsubscribe<T>` — 泛型订阅/取消
- `Fire<T>(T)` — 触发泛型事件
- `Fire(string)` — 触发无参数事件（使用 `Action` 多播委托）
- `Clear()` — 清空所有订阅

内部使用 `Dictionary<Type, Delegate>` 存储类型化处理器，通过不可变委托重组保证线程安全。

---

## 四、数据模型

### 4.1 BlockData（核心内容块）

```csharp
public class BlockData
{
    public string id;
    public string type;          // 块类型：section_title, news_item, featured_match 等
    public float marginTop;
    public float marginBottom;
    public BlockActionData action;
    public string badge;
    public string title;
    public string subtitle;
    public string source;
    public string boardTitle;
    public string fen;           // 象棋FEN串
    public string text;
    public string time;
    public string url;
    public string posterUrl;
    public string streamUrl;
    public string caption;
    public float aspectRatio = 1.6f;
    public float height = 12f;
    public int durationSeconds;
    public MediaAssetData media;
    public ReplayData replay;
}
```

### 4.2 BlockActionData

```csharp
public class BlockActionData
{
    public string type;          // none, open_article, open_match, open_ai_practice, start_ranked_match
    public string target;
    public Dictionary<string, string> parameters;
    // GetParameter(key) 方法
    // static None() 工厂方法（返回 type="none"）
}
```

### 4.3 PageData

```csharp
public class PageData
{
    public string pageId;
    public string pageType;      // Page, Article, Replay
    public string title;
    public List<BlockData> blocks;
}
```

### 4.4 FeaturePageData / FeatureSectionData / FeatureItemData

```csharp
public class FeaturePageData
{
    public string pageId;
    public string title;
    public List<FeatureSectionData> sections;
}

public class FeatureSectionData
{
    public string id;
    public string type;          // section_header, quick_action_grid, feature_action_card 等
    public string title;
    public string subtitle;
    public string actionText;
    public BlockActionData action;
    public List<FeatureItemData> items;
}

public class FeatureItemData
{
    public string id;
    public string title;
    public string subtitle;
    public string value;
    public string detail;
    public string badge;
    public string state;         // 颜色变体: accent, gold, muted
    public string icon;          // 单字符图标
    public string time;
    public string result;        // "胜"/"负"/"和"
    public bool locked;
    public BlockActionData action;
}
```

### 4.5 GameRoom 数据模型

```csharp
public enum GameRoomMode { Player, AiTraining, Spectator }
public enum GamePieceSide { Red, Black }

public class GameRoomData
{
    public string roomId;
    public string title;
    public GameRoomMode mode;
    public string roundText;
    public string statusText;
    public string countdownText;
    public int viewerCount;
    public GamePlayerData redPlayer;
    public GamePlayerData blackPlayer;
    public List<GamePieceData> pieces;           // 棋盘上32个棋子
    public List<GameCapturedPieceData> redCapturedPieces;
    public List<GameCapturedPieceData> blackCapturedPieces;
    public List<GameMoveMarkerData> moveMarkers;  // 最后一步的标记
    public List<GameRoomActionData> actions;
    public List<GameDanmakuData> danmaku;         // 弹幕
    // IsSpectator() 辅助方法
}

public class GamePlayerData
{
    public string playerId, displayName, rank, avatarText;
    public string timerText, sideText;
    public GamePieceSide side;
    public bool active, localPlayer;
}

public class GamePieceData
{
    public string pieceId, text;  // text为象棋字符："車""馬""炮"等
    public GamePieceSide side;
    public int x, y;              // 0-8, 0-9
    public bool highlighted;
}

public class GameCapturedPieceData { public string text; public GamePieceSide side; }
public class GameMoveMarkerData { public int x, y; public float alpha; }
public class GameRoomActionData { public string actionId, label, icon; }
public class GameDanmakuData { public string text, track; public float offset; }
```

### 4.6 其他数据模型

**MediaAssetData**：mediaId, type, url, posterUrl, streamUrl, mimeType, version, hash, width, height, aspectRatio, durationSeconds。

**ReplayData**：replayId, gameId("xiangqi"), initialState(FEN), initialStateFormat, startStepIndex, endStepIndex, autoPlay, loop, secondsPerStep(默认0.6), renderProfile(ReplayRenderProfileData), steps(List\<ReplayStepData\>)。方法 `ResolveEndStepIndex()`。

**ReplayStepData**：index, command(ICCS), notation(中文), comment, duration。

**ReplayRenderProfileData**：mode(canvas_2d/scene_2d/scene_3d), textureWidth(默认1024), textureHeight(默认576)。

**ArticleCommentData**：commentId, authorName, avatarText, time, text, authorReply(ArticleCommentReplyData)。

**ArticleEngagementSummaryData**：articleId, bookmarked, flowered, canComment, flowerCount, commentCount, previewComments(List\<ArticleCommentData\>)。

---

## 五、Theme 主题系统

命名空间 `NewsFramework.UI.Theme`。静态类 `AppTheme`，所有常量：

### 颜色（使用 Color32 RGB 字节构建）
- PageBackground: #F6F5F1（暖纸色）
- Surface: #FFFFFC（白色卡片表面）
- SurfaceMuted: #F4F3EF（低对比度表面）
- Hairline: #E2DFD8（分割线）
- PrimaryText: #2A2319（主文字深棕）
- SecondaryText: #7D7363（次要文字灰棕）
- Accent: #9A3430（中国红强调色）
- DarkBar: #2F2517（深色导航栏背景）
- TabInactive: #7E776D（未选中Tab颜色）

### 尺寸常量
- ScreenWidth: 375, ScreenHeight: 812
- PagePadding: 16, BottomBarHeight: 64, TopBarHeight: 56
- CardRadius: 8

### 字体加载（TextMeshPro SDF）
- `FontAsset` 属性：优先加载 `Resources.Load<TMP_FontAsset>("Font/SourceHanSansSC-Normal SDF")`，失败则 TMP_Settings.defaultFontAsset，最后 LiberationSans
- `FontAssetBold` 属性：加载 `"Font/SourceHanSansSC-Bold SDF"`，失败回退到 FontAsset
- 两个都带缓存（私有静态字段）

### 辅助方法
- `Rgb(byte r, byte g, byte b)` → new Color32(r, g, b, 255)
- `Rgba(byte r, byte g, byte b, byte a)` → new Color32(r, g, b, a)

---

## 六、UI Base 基础工厂

命名空间 `NewsFramework.UI.Base`。

### 6.1 AppUIFactory（静态工厂类）

所有 UI 元素创建的唯一入口。

**通用创建**：
- `CreateObject(name, parent)` — 创建带 RectTransform 的 GameObject，SetParent(parent, false)
- `CreateRect(name, parent)` — 同上，返回 RectTransform
- `CreateImage(name, parent, color)` — 创建带 Image 组件的对象
- `CreateButton(name, parent, background, onClick)` — 创建 Button，带 Image 背景，颜色渐变（highlightedColor = Tint(bg, 0.96)，pressedColor = Tint(bg, 0.9)）

**文字创建**（核心方法）：
```csharp
CreateText(string name, Transform parent, string text, float fontSize, Color color,
    FontStyles style = FontStyles.Normal,
    TextAlignmentOptions alignment = TextAlignmentOptions.Left)
```
- 创建 TextMeshProUGUI 组件
- 设置 fontSize, color, fontStyle, alignment
- enableWordWrapping = true, overflowMode = Ellipsis, raycastTarget = false, margin = zero
- **字体分配**：若 style 含 Bold → 使用 AppTheme.FontAssetBold；否则使用 AppTheme.FontAsset

**Canvas 创建**：
- `CreateOverlayCanvas(name)` — ScreenSpaceOverlay Canvas + CanvasScaler (375×812, MatchWidthOrHeight=0.5) + GraphicRaycaster
- `EnsureEventSystem()` — 若无 EventSystem 则创建 + StandaloneInputModule

**布局辅助**：
- `AddVerticalLayout(target, spacing, padding, alignment)` — 添加 VerticalLayoutGroup
- `AddHorizontalLayout(target, spacing, padding, alignment)` — 添加 HorizontalLayoutGroup
- `AddLayoutElement(target, preferredHeight, preferredWidth)` — 添加 LayoutElement

**锚点辅助**：
- `Stretch(RectTransform)` — 全拉伸 (0,0)→(1,1)
- `AnchorTop(RectTransform, height)` — 顶部锚定 (0,1)→(1,1)，pivot(0.5,1)
- `AnchorBottom(RectTransform, height)` — 底部锚定 (0,0)→(1,0)，pivot(0.5,0)
- `SetOffsets(RectTransform, left, top, right, bottom)` — 直接设置 offsetMin/offsetMax

**工具方法**：
- `Tint(Color, float factor)` — RGB通道分别乘以factor（clamp到0-1）

### 6.2 SafeAreaFitter

MonoBehaviour，根据 `Screen.safeArea` 自动调整 RectTransform 边距。在 Awake、OnEnable、Update 中检查并应用（当 safeArea 变化时）。支持四个边分别启用/禁用。

### 6.3 ModalOverlay（模态弹窗）

静态方法 `Show(Config, Transform parent)`：
- Config 结构：title（可选）、body、ButtonDef[] buttons
- ButtonDef：label、onClick、style（Normal/Destructive/Warning）
- 创建逻辑：全屏半透明黑色遮罩（55% alpha）+ 居中面板（284px宽）+ 垂直布局
- 标题：18pt bold；正文：14pt normal
- 按钮行：水平布局。Normal 按钮=SurfaceMuted 背景；Destructive=Accent 背景+白色文字；Warning=金色背景
- `Close(GameObject overlay)` — Destroy

### 6.4 ChessBoardRenderer（棋盘渲染器）

静态类，使用象棋棋盘纹理。

常量：DefaultBoardWidth=345, DefaultBoardHeight=384, DefaultPadding=19, DefaultPieceSize=38

- `ResolveBoardPoint(x, y, pieceSize, boardW, boardH, padding)` — 网格坐标转像素坐标（8列×9行）
- `LoadBoardSprite()` — 从 Resources/Textures/Chess/chessboard 加载 Texture2D 并创建 Sprite
- `CreateBoardBackground(parent, w, h)` — 居中棋盘背景图片
- `CreatePieceIcon(parent, pieceData, pieceSize, darkBoard)` — 创建棋子：
  - 圆形背景（棋子底色），红方文字色 #B5433A，黑方文字色 #2C2416
  - 棋子背景色：浅色棋盘为 #FFF8E7
  - 被选中棋子带金色光晕
- `CreateMoveMarker(parent, x, y, pieceSize, color, alpha)` — 8px 圆形标记点

---

## 七、Block（内容块）系统

命名空间 `NewsFramework.UI.Blocks`。

### 7.1 核心接口和基类

**IBlockView** 接口：
- `Bind(BlockData data, Action<BlockActionData> onAction)`

**BlockRenderContext**：携带 `IMediaAssetService`，静态工厂 `CreateDefault(mediaServerConfig)`。

**BlockViewBase : MonoBehaviour, IBlockView**：
- 抽象基类，存储 Data、OnAction 回调、Context
- `Bind(data, onAction)` → 存储数据后调用抽象方法 `OnBind(BlockData data)`
- `TriggerAction()` — 若 action.type 不为 "none"，调用 OnAction
- `SetContext(BlockRenderContext)` — 设置渲染上下文

### 7.2 BlockRegistry（注册表）

```csharp
public class BlockRegistry
{
    public void Register(string type, Func<Transform, BlockViewBase> factory);
    public BlockViewBase Create(BlockData data, Transform parent);  // 找不到返回 UnknownBlockView
    public static BlockRegistry CreateDefault();  // 注册全部15种块类型
}
```

### 7.3 BlockRenderer（渲染器）

MonoBehaviour：
- `Initialize(RectTransform root, BlockRegistry registry, Action<BlockActionData> onAction, BlockRenderContext context)`
- `Render(PageData pageData)` — 清空后逐个渲染 blocks
- `Append(IEnumerable<BlockData>)` — 追加不清空
- `Clear()` — 销毁所有子对象
- 每个 block 渲染时，在 block 前后添加 marginTop/marginBottom 间距

### 7.4 所有 BlockView 实现

每个 BlockView 都继承 `BlockViewBase`，具有 `static Create(Transform parent)` 工厂方法，在 `OnBind()` 中填充数据。

| 类型 | 类名 | 高度 | UI结构 |
|---|---|---|---|
| `featured_match` | FeaturedMatchBlockView | 386px | Button卡片：badge标签（accent背景白色文字）、标题23pt bold、副标题15pt、棋盘预览区192px（含棋子图标+棋盘标题）、底部栏（来源+ "查看详情>") |
| `board_preview` | BoardPreviewBlockView | 250px | Button：标题18pt bold、棋盘预览区（含42pt棋子图标+棋盘标题居中） |
| `section_title` | SectionTitleBlockView | 38px | 左侧4×21px accent竖条 + 标题21pt bold + 可选副标题13pt |
| `news_item` | NewsItemBlockView | 86px(无缩略图76px) | Button："◆ "前缀标题16pt bold(最多2行)、元信息行(source | time)、96×72可选缩略图(通过MediaAssetService异步加载)、底部分割线 |
| `live_match_item` | LiveMatchItemBlockView | 72px | Button：52×52图标方块(badge文字作图标)、文字列(标题17pt bold+副标题13pt)、accent"围观"按钮 |
| `article_tip_card` | ArticleTipCardBlockView | 118px | Button：标题19pt bold、摘要14pt(2行,lineSpacing=2)、底部栏含右对齐badge标签 |
| `lifestyle_card` | LifestyleCardBlockView | 108px | Button：84×84缩略图(异步加载) + 文字列(标题17pt bold最多2行+摘要13pt+元信息12pt) |
| `spacer` | SpacerBlockView | data.height(默认12) | 空RectTransform + LayoutElement |
| `divider` | DividerBlockView | 1px | 分割线Image(hairline色) |
| `paragraph` | ParagraphBlockView | 自适应 | TextMeshProUGUI 16pt, lineSpacing=18 |
| `image` | ImageBlockView | ~241px(自适应) | SurfaceMuted背景+RawImage(异步加载)+placeholder文字+caption 12pt居中。高度=clamp(343/aspectRatio, 140, 260) |
| `video` | VideoBlockView | ~272px | 同image + 播放三角覆盖层34pt"▶" + 格式化时长元信息 |
| `replay_preview` | ReplayPreviewBlockView | 258px | Button：标题18pt+副标题13pt+预览区142px(棋子图标+棋盘标签)+底部栏(步数标签+"查看回放>"accent色) |
| `replay` | ReplayBlockView | ~320px | 完整交互式回放：标题16pt+棋盘区(ChessSimulation驱动棋子+标记)+播放/暂停/步进控制+步数标签+进度条。使用Update()自动播放。支持seek、toggle、loop |
| `article_header` | ArticleHeaderBlockView | 124px | 标题24pt bold(最多3行) + 元信息行(source | time) 13pt secondary |
| 未知类型 | UnknownBlockView | 48px | 调试显示"Unsupported block: {type}" |

---

## 八、Feature Section（功能区块）系统

命名空间 `NewsFramework.UI.Features`。

### 8.1 核心接口和基类

**IFeatureSectionView**：`Bind(FeatureSectionData, Action<BlockActionData>)`

**FeatureSectionViewBase**：抽象基类，同 BlockViewBase 模式
- `Bind(data, onAction)` → `OnBind(FeatureSectionData data)`
- `TriggerSectionAction()`, `TriggerItemAction(FeatureItemData)`, `TriggerAction(BlockActionData)`

**FeatureSectionRegistry**：`Register(string type, factory)` + `Create(FeatureSectionData, parent)` + `CreateDefault()`

**FeaturePageRenderer : MonoBehaviour**：`Initialize(root, registry, actionHandler)` + `Render(FeaturePageData)` + `Clear()`

**FeatureViewHelpers** 静态工具类：
- `ResultColor(string result)` — "胜"=绿色, "负"=Accent, "和"=Secondary
- `CreateCard(name, parent)` — 创建Surface色Image
- `CreateCardButton(name, parent, onClick)` — 创建Surface色Button
- `CreateTitle(name, parent, text, size=18)` — 创建bold单行文字
- `CreateMeta(name, parent, text, alignment)` — 创建13pt secondary单行文字
- `HasAction(FeatureItemData)` — 检查action是否有效

### 8.2 所有 FeatureSectionView 实现

| 类型 | 类名 | UI结构 |
|---|---|---|
| `section_header` | FeatureSectionHeaderView | 30px高。3px accent竖条 + 标题20pt bold(flexible宽度) + 可选右侧action按钮(accent文字) |
| `quick_action_grid` | QuickActionGridSectionView | 150px高。水平行卡片(flexible宽度)。每卡片：30pt图标 + 18pt标题 + 13pt副标题 + badge胶囊(accent背景白色文字) |
| `feature_action_card` | FeatureActionCardSectionView | 78px高。Button：文字列(19pt标题+13pt副标题) + value标签(右侧) + action胶囊(SurfaceMuted, accent文字) |
| `recent_match_list` | RecentMatchListSectionView | 卡片内每行56px。每行："□"图标 + 14pt标题 + result badge(颜色编码胜/负/和) + time + 分割线 |
| `profile_header` | ProfileHeaderSectionView | 96px高。72px圆形头像(icon文字) + 26pt bold名字 + 13pt accent bold称号badge |
| `stats_row` | StatsRowSectionView | 72px高。水平行统计卡片(flexible宽度)。每卡片：26pt bold居中value + 13pt secondary居中title |
| `achievement_strip` | AchievementStripSectionView | 102px高。水平行成就项(72×96)。每项：54px badge圆圈(已解锁=金色,未解锁=muted) + 22pt icon文字 + 11pt标签 |
| `rank_list` | RankListSectionView | 卡片行(64px)。每行：34px图标方块(accent或dark) + 文字列(16pt标题+13pt副标题) + 16pt金色bold rank值 |
| `settings_list` | SettingsListSectionView | 卡片行(56px)。每行：Button + 16pt标题 + 14pt value(accent色若state="accent") + 分割线 |
| `about_footer` | AboutFooterSectionView | 栈布局：链接文字(14pt secondary) + version文字(12pt) + slogan(13pt)。高度由item数量计算 |
| `empty_state` | EmptyStateSectionView | 160px高。居中列：18pt bold标题 + 13pt副标题("数据模块建设中") |
| 未知类型 | UnknownFeatureSectionView | 48px。"未知功能区块: {type}" |

---

## 九、Pages 页面

命名空间 `NewsFramework.UI.Pages`。

### 9.1 TabPageView / TabPageHost

**TabPageView**：
- `Create(pageId, parent)` → 创建 ScrollRect(viewport+mask) + contentRoot(VerticalLayoutGroup, spacing=14, padding=16) + ContentSizeFitter + CanvasGroup
- `SetVisible(bool)`, `SetInteraction(bool)`, `SetAlpha(float)`
- `SetPageOffset(x)` / `ResetPageOffset()` — 横向平移动画
- `ResetScrollPosition()` — 滚回顶部

**TabPageHost : MonoBehaviour**：
- `Initialize(RectTransform root)` + `CreatePage(pageId)`
- `Show(int index, bool animate)` — 带横向滑动动画(0.18s, ease-out cubic)，管理所有page可见性，处理中断过渡
- `ActiveIndex` 属性, `IsTransitioning`

### 9.2 HomePage : MonoBehaviour（主页面）

**Build 流程**（`Build(ContentRuntimeConfig, MediaServerConfig)`）：
1. 创建 overlay Canvas + SafeAreaFitter + PageBackground 背景
2. **顶部栏** BuildTopArea()：状态栏(时间11:36 + 系统图标) + 导航栏(24pt bold标题 + 搜索按钮"⌕") + 底部hairline分割线
3. **底部Tab栏** BuildBottomTabs()：4个Tab(首页/数据/对局/我的)，每个带图标+文字，accent色选中态
4. **内容区** BuildContentArea()：TabPageHost 含4个 TabPageView(Home/Data/Match/Profile)
   - Home：BlockRenderer 渲染首页feed(通过IFeedPager)
   - Data/Match/Profile：FeaturePageRenderer 渲染各自内容(Mock数据初始化)
5. 初始化滚动监听(idle后加载)

**Action 处理** `HandleAction(BlockActionData)`：
- `open_article` → ShowArticle(articleId)：创建ArticlePage
- `open_match` + target="match_live" → ShowSpectatorRoom
- `open_ai_practice` / `start_ranked_match` → ShowPlayerRoom
- ShowGameRoom/CloseGameRoom：创建/销毁GameRoomPage

**Feed 分页**：
- `HandleFeedPageLoaded(FeedPageResult)` → 判断reset/append
- `EnsureFeedBuffer()` → 当剩余像素 <= viewportHeight × prefetchScreens 时触发预加载
- Tab 切换时处理脏标记和延迟刷新

### 9.3 ArticlePage : MonoBehaviour（文章详情页）

**Build 参数**：parent, contentService, articleId, onBack, actionHandler, knownVersion, renderContext, engagementService

**流程**：
1. 创建背景、顶部栏（返回按钮"‹" + 标题 + 收藏切换☆/★）、滚动内容区、底部评论输入栏
2. 初始化 BlockRenderer 渲染文章内容
3. 订阅 `contentService.PageUpdated` 事件实现实时更新
4. 调用 `contentService.LoadPage(ContentRequest.Article(articleId))`

**文章底部**：结束标记"◆" + 互动区（点赞提示 + 花朵按钮带计数 + 分享按钮）

**评论系统**：从 engagementSummary 渲染预览评论，含头像圆圈 + 作者名 + 时间 + 文本 + 可选作者回复

**互动**：收藏切换、送花（带计数、单次限制）、分享、打开全部评论、打开评论输入。管理 engagement 请求版本以处理过期回调。

---

## 十、GameRoom 对局室

命名空间 `NewsFramework.UI.GameRoom`。

### 10.1 GameRoomStyle

木纹色调色板（静态类，全为 Color）：
- Walnut, WalnutDark(#2F2517), WalnutDeep
- BoardWarm, BoardDeep, BoardLineDark
- Gold(#D4A574), GoldMuted
- PieceCream(#FFF8E7), PanelLight
- RedPiece(#B5433A), BlackPiece(#2C2416)
- `Alpha(Color, float)` 辅助方法
- 常量：BoardWidth=345, BoardHeight=384

### 10.2 GameRoomUi

内部辅助：`CreatePanel`, `CreateTextButton`, `CreateLabel`, `CreateAvatar`(圆形头像+首字符+自适应字号), `SetRect`

### 10.3 ChessBoardView : MonoBehaviour

`Build(parent, data, darkBoard)`：
- 创建棋盘 RectTransform + 棋盘sprite背景
- 通过 ChessBoardRenderer 创建每个棋子和移动标记
- 棋子为Button，带边框(红/黑色调)，支持highlighted状态(金色光晕)

### 10.4 GameRoomPlayerViews（静态方法）

- `BuildTrainingOpponentPanel(parent, data)` — 68px面板：对手头像+名字+段位, 回合badge(94px宽), 倒计时(30pt)
- `BuildTrainingPlayerPanel(parent, data)` — 76px面板：同布局无回合badge
- `BuildSpectatorVersusBar(parent, data)` — 72px面板：左红方身份+中间比赛信息(回合+倒计时)+右黑方身份
- 内部复用：BuildIdentity, BuildAvatarWithActive, BuildNameColumn, BuildRoundBadge, BuildTimer

### 10.5 GameRoomActionBarView（静态方法）

- `BuildPlayerActions(parent, data, onAction)` — 88px水平操作栏(求和/认输/悔棋/复盘)
- `BuildSpectatorActions(parent, data, onAction)` — 110px：评论输入+发送+花朵按钮带计数, 观众操作链接(回放/分享/设置)

### 10.6 GameRoomOverlayViews（静态方法）

- `BuildCapturedPieces(boardSlot, data)` — 左右两侧被吃棋子列，18px圆形chip
- `BuildDanmaku(boardRoot, data)` — 浮动弹幕标签，按track/offset定位
- `BuildStatusBanner(parent, text)` — 金色badge状态横幅

### 10.7 GameRoomPage : MonoBehaviour

`Build(parent, data, backHandler)`：

**Player模式**：
- Walnut背景 + 垂直布局
- 顶部栏(返回按钮含退出确认) + 标题
- 对手面板 + 棋盘(500px, 浅色棋盘) + 本地玩家面板
- 状态横幅 + 操作栏

**Spectator模式**：
- 顶部栏(返回+观众人数胶囊)
- 对阵栏 + 棋盘(566px, 深色棋盘, 被吃棋子+弹幕)
- 观众操作栏

**退出确认弹窗**："对局还在进行中，确定离开？"，含留下/离开按钮。

### 10.8 GameResultModal（静态弹窗）

- `ResultConfig`：winnerLabel, redPlayer, blackPlayer, totalRounds, timeUsed, onGenerateReport, onRematch, onGoHome
- `ReportConfig`：reportTitle, reportSubtitle, imageUrl, onShareWechat, onRematch, onGoHome
- 结果面板：奖杯"🏆"+winnerLabel(28pt)+对局信息+统计数据+"生成AI战报"按钮(destructive)+"再来一局"(outline)+"返回首页"(link)
- `ShowReport(overlay, config)` — 过渡到战报面板：状态"战报已生成"+关闭按钮+标题(20pt)+副标题+截图区(160px)+"分享到微信"按钮(微信绿#07C160)+提示文字
- 按钮辅助：BuildVerticalButton, BuildOutlineButton

---

## 十一、Mock Data

命名空间 `NewsFramework.Data.Mock`。提供所有开发阶段的 mock 数据。

### HomeMockData
- `Create()` — 首页PageData：featured_match + section_title"此刻棋坛"(2 live_match_items) + section_title"赛事快讯"(3 news_items) + section_title"棋艺干货"(2 article_tips) + section_title"棋味生活"(lifestyle_card)
- `CreatePaged(int offset, int limit, out int total)` — 分页feed数据

### ArticleMockData
- `Create(articleId)` — 文章PageData：article_header + 多段paragraph("本轮焦点战..."、"第15回合成为转折点...") + image占位 + replay block(引用ReplayMockData) + board_preview + 尾段
- article_001：张三的棋局分析；article_002：王天一快棋名局

### MatchMockData
- `Create()` — FeaturePageData：quick_action_grid(每日残局+AI练习) + section_header"与人对弈" + feature_action_card(排位赛,业余五段,1850天梯) + feature_action_card(好友对战) + section_header"最近对局" + recent_match_list(4条记录：张三vs李四胜, 王老五vs赵六负, 陈大爷vs周老板和, 老李vs小王胜)

### ProfileMockData
- `Create()` — FeaturePageData：profile_header(张三,棋坛名将) + stats_row(对局1283, 胜率62%, 连胜7) + section_header"我的称号与成就" + achievement_strip(4成就：百胜将军/棋坛名将已解锁, 独孤求败/国手风范锁定) + section_header"我的段位" + rank_list(中国象棋业余五段1850, 围棋业余六段2100) + section_header"对局设置" + settings_list(棋盘风格/走子音效/默认时间) + about_footer(v1.0.0/隐私设置/关于我们)

### FeatureMockData
- `CreateDataPage()` — empty_state"数据模块建设中"

### ReplayMockData
- `CreateArticlePreview()` — 3步棋谱(炮二平五, 马8进7, 马二进三), autoPlay=false, canvas_2d 1024×576

### GameRoomMockData
- `CreatePlayerRoom()` — AI训练室，32棋子标准开局，红方张三(业余五段,localPlayer), 黑方李四(AI,业余七段), 操作：求和/认输/悔棋/复盘
- `CreateSpectatorRoom(matchId)` — 观战室，中局棋子，23观众，双方被吃棋子，移动标记，3条弹幕

---

## 十二、Services 服务层

### 12.1 内容服务

**接口** `IContentService`：
- `event Action<ContentResult> PageUpdated`
- `LoadPage(ContentRequest, Action<ContentResult>)`
- `RefreshPage(ContentRequest, Action<ContentResult>)`

**ContentRequest**：pageId, pageType, contentKind(Page/Article/Replay), knownVersion, refreshMode, parameters字典。静态工厂：`Page(id)`, `Article(id)`, `Replay(id)`。

**ContentResult**：success, error, page(PageData), metadata(ContentCacheMetadata), fromCache, changed。静态工厂：`Success(...)`, `Failure(...)`。

**CachedContentService**（核心实现）三层缓存：
1. MemoryContentStore（内存 Dictionary，最快）
2. IContentLocalRepository（本地持久化）
3. IContentRemoteRepository（网络）
- 缓存策略：page TTL=2h, article TTL=24h, replay TTL=7天
- 按 refreshMode 决定请求顺序
- `PageUpdated` 事件仅在 changed=true 时触发（后台更新通知）

**ContentServiceFactory**：多个工厂方法创建不同配置的服务实例。
- `CreateMockCachedService()` — 全Mock内存
- `CreateHttpCachedService(config)` — 内存+HTTP远程（带重试）
- `CreateSQLiteCachedService(path, config)` — SQLite持久化+HTTP远程

### 12.2 Feed 分页服务

**IFeedPager**：
- `Start(FeedPagerConfig, Action<FeedPageResult>)`
- `LoadNext(Action<FeedPageResult>)` — 加载下一页
- `EnsureAhead(float remainingPixels, float viewportHeight, Action<FeedPageResult>)` — 预判是否需要预加载
- IsActive, IsLoading, HasMore, FeedVersion, LoadedBlocks

**FeedPager** 实现：
- 维护 loadedBlocks 列表和 loadedBlockIds 去重集合
- 检测 feedVersion 变化 → reset模式（清空已加载）
- Dedup：按 block.id（无id则生成GUID）
- 先查本地缓存，后请求远程

**FeedPagerConfig**：feedId="home", pageSize=6, prefetchScreens=2.5f, activeOnStart=true

### 12.3 文章互动服务

**IArticleEngagementService**：`LoadSummary(articleId, Action<ArticleEngagementSummaryData>)`

**MockArticleEngagementService**：article_001 返回128花+8评论(3预览含作者回复)；article_002 返回76花+3评论；article_003 返回42花+0评论。

**RemoteArticleEngagementService**：HTTP请求，含 JSON DTO 映射层。

### 12.4 媒体服务

**IMediaAssetService**：
- `LoadTexture(MediaAssetRequest, Action<MediaAssetResult>)`
- `ResolveVideoSource(MediaAssetRequest, Action<VideoSourceResult>)`

**MediaAssetRequest**：mediaId, mediaType, url, posterUrl, streamUrl, version, hash, mimeType, aspectRatio, durationSeconds。静态工厂 `Image(BlockData)`, `Video(BlockData)`。方法 `ResolveCacheKey(serverConfig)`。

**UnityMediaAssetService** 实现：
- LoadTexture：检查缓存 → mock://URL生成程序化纹理(64×40斜条纹) → 真正URL用UnityWebRequestTexture → 写入缓存
- ResolveVideoSource：通过 MediaServerConfig 解析所有URL

**MediaServerConfig**：enabled, baseUrl("http://localhost:5234"), resolveRelativeUrls。`ResolveUrl(url)` — 相对路径补全，跳过 mock:// 和绝对URL。

**MediaAssetServiceFactory**：`CreateDefault(config)` 创建带 MemoryMediaAssetCache 的服务。

### 12.5 ContentRuntimeServices / ContentRuntimeConfig

**ContentRuntimeServices**：聚合 ContentService, FeedPager, ArticleEngagementService, CacheMaintenance, Database(ISqliteDatabase)。`Dispose()` 关闭数据库。

**ContentRuntimeMode** 枚举：Mock, Http, SQLiteMock, SQLiteHttp

**ContentRuntimeConfig**（Serializable）：mode, httpConfig, sqliteDatabasePath, runCacheMaintenanceOnStart, cacheMaintenancePolicy, feedId, feedPageSize=6, feedPrefetchScreens=2.5f, feedActiveOnStart

**ContentRuntimeServiceFactory.Create(config)** → 按 mode 创建对应服务组合。

### 12.6 SQLite 持久化

**ISqliteDatabase**：Open, Close, ExecuteNonQuery, ExecuteQuery, GetConnectionString

**SqliteDatabase**：基于 SQLite4Unity3d 的封装实现。

**SqliteDatabaseFactory**：`CreateDefault(path)` 或 `ResolveDefaultPath()`（Application.persistentDataPath/newsframework.db）。

**SqliteContentSchema**：定义内容缓存和feed缓存表结构。

**SqliteContentCacheMaintenance**：`GetStats()` 查询统计，`Cleanup(policy, callback)` 清理过期条目。

**SqliteSmokeTest**：建表→写入→读取→删除 全流程验证。

---

## 十三、Simulation 模拟/仿真层

命名空间 `NewsFramework.Simulation`。

### 13.1 核心接口

**IGameSimulation : IDisposable**：
- SimulationId, GameId, CurrentTick, IsLoaded
- `Load(GameSimulationConfig)` — 加载初始状态
- `EnqueueCommand(GameCommandData)` — 入队命令
- `Tick(GameTickInput)` → GameTickResult — 推进一帧
- `ApplySnapshot(GameSnapshotData)` / `CreateSnapshot()` — 快照
- `Reset()`

**GameSimulationConfig**：gameId="xiangqi", tickRate=20, deterministic=true, initialState(FEN), initialStateFormat="fen"。`TickDeltaSeconds` = 1f/tickRate。

**GameTickInput**：tick, deltaTime, commands(List\<GameCommandData\>)

**GameTickResult**：tick, success, error, snapshot(GameSnapshotData), events(List\<GameSimulationEvent\>)

**GameSimulationEvent**：tick, type, payload, parameters字典

**GameCommandData**：tick, sequence, playerId, type, payload, parameters字典。`GetParameter(key)`。

**GameSnapshotData**：gameId, tick, stateFormat, state(FEN串), metadata字典

**NullGameSimulation**：桩实现，存储config/snapshot，排空commands不执行逻辑。

**GameSimulationFactory**：`Create(config)` → "xiangqi"返回ChessSimulation，其他返回NullGameSimulation。

### 13.2 象棋引擎

命名空间 `NewsFramework.Simulation.Chess`。

**ChessTypes**：
- `ChessSide` 枚举：Red, Black
- `ChessPieceType` 枚举：King, Advisor, Bishop, Knight, Rook, Cannon, Pawn
- `ChessPiece` readonly struct：Side, Type, IsValid, `ToChar()`(返回中文字符：帅/仕/相/馬/車/炮/兵/將/士/象/馬/車/砲/卒), Equals/GetHashCode
- `ChessPos` readonly struct：Row(0-9), Col(0-8), IsValid, IsRedSide(0-4), IsBlackSide(5-9), IsInPalace, ManhattanDistance
- `ChessMove` readonly struct：From, To, Piece, Captured(Piece), IsCapture

**ChessBoard**（10×9棋盘）：
- `CreateStandard()` — 标准开局
- `FromFen(fen)` / `Clone()`
- 索引器 `this[row, col]` / `GetPiece(pos)` / `GetKingPos(side)`
- `SideToMove` 属性（默认红先）
- `MakeMove(from, to)` → ChessMove（含被吃子信息）
- `UndoMove(ChessMove)`
- `ToFen()` / `ToAsciiBoard()`
- FEN标准串：`rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w`
- 内部维护king位置、king存活状态、吃子信息

**ChessRuleEngine**（纯静态方法）：
- `GenerateLegalMoves(board)` — 生成所有合法走法（pseudo-move → 过滤被将军状态）
- `IsInCheck(board, side)` — 检测对方所有棋子的raw move是否威胁king + 检测将帅对面
- `IsCheckmate(board, side)` — 无合法走法 = 将死
- `IsMoveLegal(board, from, to)` — 目标在raw moves中 + 走子后不使己方被将
- `IsDrawByInsufficientMaterial(board)` — 双方各≤1子
- `KingsAreFacing(board)` — 同列无阻挡
- 各子力走法生成：King(九宫，正交1步), Advisor(九宫，斜1步), Bishop(田字对角，有象眼，不过河), Knight(日字，有蹩脚), Rook(直线到阻挡), Cannon(直线走，隔一子吃), Pawn(前进；过河后可横移)

**ChessGameRecorder**（棋谱记录器）：
- `Record(ChessMove)`, `UndoLast()`, `MoveCount`, `LastMove`, `Moves`
- `MoveToIccs(move)` → "a9a7" 坐标记法
- `IccsToMove(board, iccs)` → 解析回 ChessMove
- `MoveToChinese(move)` → "炮二平五" 中文记法
- `ToString()` → 完整中文棋谱

**ChessSimulation : IGameSimulation**：
- 内部维护 ChessBoard + ChessGameRecorder
- `Load(config)` → 从FEN解析或创建标准开局
- `Tick(input)` → 应用input.commands中的走法命令、记录走子、生成 move/error/game_over 事件、检测将死/无子
- `TryMakeMove(from, to, out move, out error)` → 合法性检查→执行→记录
- `UndoLastMove(out undone)` → 撤销最后一步
- `GetBoard()`, `GetRecorder()`, `CurrentSide` → 对外只读

**ChessAiService**：
- 棋子价值表：将=100000, 车=900, 炮=450, 马=400, 象/士=200, 兵=100(+40过河)
- `Evaluate(board)` → 从红方视角计算子力差
- `SearchBestMove(board, side, depth=2)` → 极小化极大搜索，depth层评估

### 13.3 FrameSync 帧同步

**FrameSyncConfig**：sessionId, localPlayerId, tickRate=20, inputDelayTicks=2, maxRollbackTicks=0

**IFrameSyncSession : IDisposable**：
- SessionId, State(FrameSyncState枚举), ConfirmedTick, LocalTick
- `Start()`, `SubmitLocalCommand(GameCommandData)`, `SubmitRemoteBatch(FrameCommandBatch)`
- `TryGetCommandsForTick(int, out List<GameCommandData>)`
- `ConfirmTick(int)`, `Pause()`, `Resume()`, `Stop()`

**NullFrameSyncSession**：内存字典存储frame batches，支持本地命令提交(含input delay)、远程batch提交、tick确认。

---

## 十四、Replay 回放系统

命名空间 `NewsFramework.Replay`。

**IReplayRuntime : IDisposable**：
- RuntimeId, State, Data(ReplayData), Simulation(IGameSimulation), Texture(RenderTexture), CurrentStepIndex
- `Load()`, `BindSimulation()`, `Play()`, `Pause()`, `Stop()`, `Seek(int)`, `Tick(float)`

**ReplayRuntimeState** 枚举：Empty → Loaded → Playing/Paused/Stopped → Disposed/Error

**ReplayRuntimeFactory**：`Create(ReplayData)` → 目前返回 NullReplayRuntime（占位实现）

**NullReplayRuntime**：
- 创建 RenderTexture(分辨率从data解析，默认1024×576)
- 通过 GameSimulationFactory 创建 simulation
- 管理状态机转换
- `Tick(deltaTime)` 有意留空
- `ClearTexture()` 用 GL.Clear 绘制深棕色

---

## 十五、Bootstrap 启动流程

**HomeDemoBootstrap**（MonoBehaviour）：
- `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)]` 自动创建自身
- `Start()` 中创建 AppBootstrap 组件并传入 ContentRuntimeConfig

**AppBootstrap**（MonoBehaviour）：
- `Start()` 中：
  1. 创建 EventBus 单例 → `AppBootstrap.EventBus` 静态属性
  2. 通过 ContentRuntimeServiceFactory.Create(config) 创建 ContentRuntimeServices → `AppBootstrap.Services` 静态属性
  3. 创建 HomePage 组件 → 调用 `Build(config, mediaConfig)`

---

## 十六、Editor 编辑器工具

命名空间 `NewsFramework.Editor`。

### HomeDemoSceneBuilder
- 菜单项 `NewsFramework/Build Home Demo Scene` → 创建 HomeDemo.unity 场景，添加 HomeDemoBootstrap (Mock模式)
- 菜单项 `NewsFramework/Build Content.Api Smoke Scene` → 创建 HomeDemoContentApi.unity，指向 localhost:5235/5234

### UIPreviewWindow
- 编辑器窗口 `NewsFramework/UI Preview`
- 左侧边栏：可折叠分类（Pages、Blocks 15种、Feature Sections 11种、Modals 5种）
- 右侧预览面板（深色背景）
- 选中组件后创建 OverlayCanvas + 实例化该组件
- `CreateMockBlockData(type)` 方法为每种 block 类型生成包含象棋主题数据的 BlockData
- `CreateMockSectionData(type)` 方法委托给各 MockData 类
- Pages：HomePage, ArticlePage, GameRoomPage(Player/Spectator)
- Modals：ModalOverlay(Normal/Destructive/Warning), GameResultModal(Result/Report with transitions)

### 诊断菜单
- `NewsFramework/Diagnostics/Run SQLite Smoke Test`
- `NewsFramework/Diagnostics/Print SQLite Cache Stats`
- `NewsFramework/Diagnostics/Cleanup SQLite Cache`

---

## 十七、App Server 后端

位于 `app-server/`，两个 ASP.NET Core 8.0 最小 API。

### Content API（端口 5235）

端点在 `ContentEndpoints` 中映射：
- `GET /api/feed/home?cursor=&limit=&knownVersion=` → FeedPageResult
- `GET /api/articles/{articleId}?knownVersion=` → PageData（含 blocks 数组）
- `GET /api/articles/{articleId}/engagement-summary` → ArticleEngagementSummaryData

InMemoryContentRepository：3篇硬编码文章（棋坛开局、实战复盘、训练计划），每篇含 image cover block + paragraph block。Feed 将文章包装为 article_card blocks，含 media 引用。engagement 含 article_001 的 1 条预览评论。

Contracts（镜像 Unity 侧数据契约）：PageData, BlockData, MediaAssetData, FeedPageResult, ArticleEngagementSummaryData, CommentPreviewData, ApiResponse\<T\>。

### Media API（端口 5234）

端点在 `/api/media` 下：
- `POST /upload-intent` → 创建媒体资源 + 上传会话
- `PUT /uploads/{uploadId}/bytes` → 原始字节上传
- `POST /complete` → 完成上传，创建原始变体
- `GET /{mediaId}` → 元数据
- `GET /{mediaId}/variants` → 变体列表
- `GET /variants/{variantId}/content` → 流式文件内容

领域模型：MediaAsset, MediaAssetStatus(AwaitingUpload/Processing/Ready/Error), MediaUploadSession, MediaVariant, MediaVariantKind(Original)

持久化：PersistentMediaRepository(JSON → data/media-metadata/repository.json) + LocalMediaObjectStorage(data/media-objects/{id}/original/{filename})

---

## 十八、设计图参考（设计图/）

8个 HTML mockup 使用 Tailwind CSS 渲染在手机模拟框(375×812)中：

| 文件 | 内容 |
|---|---|
| 首页.html | 完整首页：状态栏、导航栏"今日棋坛"、精选赛事卡片(含SVG棋盘棋子)、live match列表、news列表(菱形项目符号)、tips卡片("AI生成"标签)、生活方式卡片、底部Tab栏 |
| 首页.jpeg | 首页渲染截图 |
| 对局.html | Match Tab界面 |
| 我的.html | Profile Tab界面 |
| 文章详情.html | 文章详情页 |
| 观战.html | 观战界面 |
| 下棋.html | 对局界面 |
| 对局结束弹窗.html | 对局结果弹窗 |
| 对局中弹窗.html | 退出确认弹窗 |

设计字体：Noto Sans SC（标题/正文），Noto Serif SC（辅助），STKaiti（棋盘文字），JetBrains Mono（代码）

---

## 十九、关键架构原则

1. **零 Prefab 策略**：所有 UI 通过 `GameObject.AddComponent<T>()` 程序化构建，绝对不使用 .prefab 文件
2. **统一入口**：所有文字通过 `AppUIFactory.CreateText()` 创建，所有字体通过 `AppTheme.FontAsset/FontAssetBold` 控制
3. **注册表模式**：Block 和 FeatureSection 通过字符串 key → 工厂函数的注册表创建，可扩展
4. **单向数据流**：Data → Renderer → View.Bind() → OnBind() → Unity GameObject
5. **Mock/生产分离**：通过接口隔离，MockImplementation + 生产Implementation 共存，ContentRuntimeMode 配置切换
6. **三层缓存**：Memory(快) → SQLite(持久) → HTTP(网络)，带 TTL 和版本检测
7. **命名空间 = 目录路径**：`NewsFramework.UI.Blocks` 对应 `UI/Blocks/`
8. **无注释**：代码通过良好的命名自解释，不写注释
9. **安全操作**：按钮/文字设置 raycastTarget=false 避免不必要的射线检测
