# Feature Page Schema

## 目标

`FeaturePageData` 用于描述功能型页面，例如 `对局`、`数据`、`我的`。

它和 `BlockData/PageData` 的边界不同：

- `BlockData/PageData`：内容流、文章详情、图文混排、回放内容块。
- `FeaturePageData`：固定业务功能页、工具入口、个人中心、设置、榜单等应用功能界面。

功能页仍然数据驱动，但不强行套入文章 block schema，避免内容协议承担业务页面布局职责。

## 数据结构

```csharp
FeaturePageData
  pageId
  title
  sections[]

FeatureSectionData
  id
  type
  rendererKey
  prefabKey
  fallbackType
  title
  subtitle
  actionText
  action
  items[]

FeatureItemData
  id
  title
  subtitle
  value
  detail
  badge
  state
  icon
  time
  result
  locked
  action
```

`FeatureSectionData.type` 由 `FeatureSectionRegistry` 映射到具体 section view。

## 当前 Section Types

- `section_header`
- `quick_action_grid`
- `feature_action_card`
- `recent_match_list`
- `profile_header`
- `stats_row`
- `achievement_strip`
- `rank_list`
- `settings_list`
- `settings_list_prefab`
- `about_footer`
- `empty_state`

## Prefab Section Rendering

`rendererKey == "prefab"` or a non-empty `prefabKey` selects
`PrefabFeatureSectionView`. The prefab is loaded with
`Resources.Load<GameObject>(prefabKey)` and must expose
`IDataBoundView<FeatureSectionData>` on one of its root components. When the
prefab is missing or the binding component is invalid, `fallbackType` points to
the code-rendered section used as the safe fallback.

```text
FeatureSectionData.rendererKey/prefabKey
  -> FeatureSectionRegistry.RegisterPrefab or inline prefabKey
  -> PrefabFeatureSectionView
  -> Resources.Load(prefabKey)
  -> IDataBoundView<FeatureSectionData>.Bind(data, onAction)
```

未知类型会渲染为 `UnknownFeatureSectionView`，用于暴露服务端或本地配置错误。

## 运行链路

```text
FeaturePageData
  -> FeaturePageRenderer
  -> FeatureSectionRegistry
  -> FeatureSectionViewBase
  -> uGUI runtime nodes
```

`FeaturePageRenderer` 不依赖 prefab。每个 section view 运行时创建 `Image/Text/Button/LayoutGroup` 等 uGUI 节点。

## 当前接入

- `HomePage` 仍然负责 app shell：顶部栏、内容滚动区、底部 tab。
- `TabPageHost` 负责 tab 页面容器、横向切换动画和 active/inactive 状态。
- 每个 `TabPageView` 持有独立 `ScrollRect` 和 `ContentRoot`，切换时保留滚动位置和已创建节点。
- 首页 tab 使用 `IFeedPager + BlockRenderer`。
- 数据/对局/我的 tab 使用 `FeaturePageRenderer`。
- 离开首页时暂停 feed 预取。
- feed 回调在首页非激活时只更新 pager 缓存，不覆盖当前功能页。
- 切回首页时如果 feed 数据有更新，才用 `IFeedPager.LoadedBlocks` 重建首页内容。
