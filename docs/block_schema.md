# Block Schema

## 定义

Block schema 是页面内容协议。它把页面拆成多个内容块，并用结构化数据描述每个内容块的类型、字段和行为。

Unity 端根据 `type` 决定使用哪个 UI 组件渲染该 block。

## 页面数据结构

首版页面数据结构：

```json
{
  "pageId": "home",
  "pageType": "home",
  "title": "今日棋坛",
  "blocks": []
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `pageId` | string | 是 | 页面唯一标识 |
| `pageType` | string | 是 | 页面类型 |
| `title` | string | 否 | 页面标题 |
| `blocks` | array | 是 | 内容块列表 |

## Block 通用字段

所有 block 都应支持以下通用字段：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `id` | string | 是 | block 唯一标识 |
| `type` | string | 是 | block 类型 |
| `rendererKey` | string | 否 | 可选渲染器选择键；首个 prefab 通路使用 `prefab` |
| `prefabKey` | string | 否 | prefab 资源键；当前骨架通过 `Resources.Load<GameObject>(prefabKey)` 加载 |
| `fallbackType` | string | 否 | prefab 缺失或无法绑定时的降级类型提示 |
| `marginTop` | number | 否 | 顶部间距 |
| `marginBottom` | number | 否 | 底部间距 |
| `action` | object | 否 | 点击行为 |

## 混合渲染字段

首版混合渲染不替换现有代码生成卡片，只在 `BlockRegistry` 增加 prefab
路径：

```json
{
  "id": "game_entry_prefab_001",
  "type": "game_entry_card",
  "rendererKey": "prefab",
  "prefabKey": "Prefabs/Blocks/GameEntryCard",
  "fallbackType": "featured_match",
  "title": "今日人机挑战",
  "action": {
    "type": "open_ai_practice",
    "target": "xiangqi"
  }
}
```

运行规则：

- `rendererKey == "prefab"` 或 `prefabKey` 非空时，`BlockRegistry` 选择
  `PrefabBlockView`。
- 代码也可以调用 `BlockRegistry.RegisterPrefab(type, prefabKey, fallbackType)`
  为某个 block type 注册默认 prefab。
- prefab 根节点必须挂载实现 `IDataBoundView<BlockData>` 的组件；组件只做
  数据绑定和本地输入转 action，不直接处理导航或业务服务。
- prefab 缺失或没有绑定组件时显示 fallback 占位并输出 warning，不导致页面
  崩溃。

当前 mock feed 中 `live_match_001` 已使用 `game_entry_card` + prefab 通路；
`live_match_002` 仍使用 `live_match_item` 代码 View，作为同一 feed 内的对照。

## Editor Prefab Scaffolder

`NewsFramework/Prefab Scaffolder/BlockData To Prefab` opens an Editor-only
tool for creating a first-pass prefab from a `BlockData` sample:

```text
select block type
  -> load built-in sample data
  -> preview a temporary scaffold
  -> export a prefab under Assets/RuntimeResources/Resources/Prefabs/Blocks/Generated
  -> validate root IDataBoundView<BlockData>
```

The exported scaffold uses `GenericBlockPrefabView` on the prefab root. Designers
can then adjust the prefab asset, while runtime continues to load it through
`prefabKey` and bind via `IDataBoundView<BlockData>`.

## 媒体引用

文章内容 JSON 不携带图片、视频等二进制内容，只携带媒体引用。客户端先渲染文本和布局，再由媒体服务异步加载资源。

通用媒体对象：

```json
{
  "mediaId": "media_001",
  "type": "image",
  "url": "https://cdn.example.com/articles/001/img1.webp",
  "posterUrl": "",
  "streamUrl": "",
  "mimeType": "image/webp",
  "version": "media_001_v1",
  "hash": "sha256:xxx",
  "width": 1200,
  "height": 760,
  "aspectRatio": 1.58,
  "durationSeconds": 0
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `mediaId` | string | 否 | 媒体唯一标识 |
| `type` | string | 是 | `image`、`video`、`audio` |
| `url` | string | 否 | 图片等直接资源 URL |
| `posterUrl` | string | 否 | 视频封面 URL |
| `streamUrl` | string | 否 | 视频流地址，例如 HLS |
| `mimeType` | string | 否 | MIME 类型 |
| `version` | string | 否 | 媒体版本，用于缓存 key |
| `hash` | string | 否 | 资源 hash，用于去重和校验 |
| `width` | number | 否 | 原始宽度 |
| `height` | number | 否 | 原始高度 |
| `aspectRatio` | number | 否 | 展示宽高比 |
| `durationSeconds` | number | 否 | 视频/音频时长 |

## Action

首版支持简单 action：

```json
{
  "type": "open_page",
  "target": "match_detail",
  "params": {
    "matchId": "match_001"
  }
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `type` | string | 是 | 行为类型 |
| `target` | string | 否 | 目标页面或目标功能 |
| `params` | object | 否 | 行为参数 |

首版 action 类型：

- `open_page`
- `open_article`
- `open_match`
- `none`

## 首版 Block 类型

### featured_match

首页重点对局卡片。

```json
{
  "id": "featured_001",
  "type": "featured_match",
  "badge": "本日最佳对局",
  "title": "张三 15回合绝杀 李四",
  "subtitle": "屏风马破当头炮 · 午间挑战赛",
  "source": "中国象棋",
  "boardTitle": "终局局面",
  "fen": "mock-fen",
  "action": {
    "type": "open_match",
    "target": "match_detail",
    "params": {
      "matchId": "match_001"
    }
  }
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `badge` | string | 否 | 卡片角标 |
| `title` | string | 是 | 对局标题 |
| `subtitle` | string | 否 | 副标题 |
| `source` | string | 否 | 来源 |
| `boardTitle` | string | 否 | 棋盘区域标题 |
| `fen` | string | 否 | 棋局局面数据，首版可为 mock |

### board_preview

棋盘或局面预览。

```json
{
  "id": "board_001",
  "type": "board_preview",
  "title": "终局局面",
  "fen": "mock-fen",
  "action": {
    "type": "open_match",
    "target": "match_detail",
    "params": {
      "matchId": "match_001"
    }
  }
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `title` | string | 否 | 棋盘说明 |
| `fen` | string | 否 | 棋局局面数据 |

首版可先渲染为占位区域，不实现完整棋盘。

### replay_preview

列表流里的轻量回放入口，只用于首页、频道页、搜索结果等 feed 场景。它不创建 `IReplayRuntime`，不申请 `RenderTexture`，不提供播放控制。

```json
{
  "id": "replay_preview_001",
  "type": "replay_preview",
  "title": "上海队关键三步回放",
  "subtitle": "第12至14回合 · 中路抢攻",
  "boardTitle": "点击进入文章查看完整回放",
  "replay": {
    "replayId": "replay_article_001",
    "gameId": "xiangqi",
    "initialState": "mock-fen",
    "initialStateFormat": "fen",
    "startStepIndex": 12,
    "endStepIndex": 18,
    "steps": [
      {
        "index": 12,
        "command": "move:cannon_2_5",
        "notation": "炮二平五"
      }
    ]
  },
  "action": {
    "type": "open_article",
    "target": "article_detail",
    "params": {
      "articleId": "article_001"
    }
  }
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `title` | string | 是 | 预览标题 |
| `subtitle` | string | 否 | 预览说明 |
| `boardTitle` | string | 否 | 棋盘占位区域文字 |
| `replay` | object | 否 | 轻量回放摘要，可只包含起止步和少量 steps |
| `action` | object | 是 | 点击后进入文章或回放详情 |

约束：

- feed/list 页面使用 `replay_preview`。
- `replay_preview` 不播放、不 tick、不持有 runtime。
- 完整播放控制只放在详情页的 `replay` block。

### replay

文章内游戏回放块。首版只定义数据结构和运行时接口，不实现真实帧同步和游戏规则。

```json
{
  "id": "replay_001",
  "type": "replay",
  "title": "关键回放",
  "subtitle": "第12步至第18步",
  "aspectRatio": 1.777,
  "replay": {
    "replayId": "replay_article_001",
    "gameId": "xiangqi",
    "initialState": "mock-fen",
    "initialStateFormat": "fen",
    "startStepIndex": 12,
    "endStepIndex": 18,
    "autoPlay": false,
    "loop": false,
    "secondsPerStep": 0.6,
    "renderProfile": {
      "mode": "canvas_2d",
      "textureWidth": 1024,
      "textureHeight": 576,
      "cameraProfile": "article_preview",
      "background": "paper"
    },
    "steps": [
      {
        "index": 12,
        "command": "move:cannon_2_5",
        "notation": "炮二平五",
        "comment": "红方抢占中路。",
        "duration": 0.6
      }
    ]
  }
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `title` | string | 否 | 回放标题 |
| `subtitle` | string | 否 | 回放说明 |
| `aspectRatio` | number | 否 | 展示区域宽高比 |
| `replay` | object | 是 | 回放数据 |

`replay` 字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `replayId` | string | 是 | 回放唯一标识 |
| `gameId` | string | 是 | 游戏类型，例如 `xiangqi` |
| `initialState` | string | 是 | 初始局面 |
| `initialStateFormat` | string | 是 | 初始局面格式，例如 `fen` |
| `startStepIndex` | number | 是 | 回放起始步 |
| `endStepIndex` | number | 否 | 回放结束步，缺省表示最后一步 |
| `autoPlay` | boolean | 否 | 是否自动播放 |
| `loop` | boolean | 否 | 是否循环播放 |
| `secondsPerStep` | number | 否 | 默认每步时长 |
| `renderProfile` | object | 是 | 渲染配置 |
| `steps` | array | 是 | 步骤列表 |

约束：

- `replay` 只用于文章详情、回放详情等 detail 页面。
- feed/list 页面不直接下发 `replay`，应下发 `replay_preview`。
- `replay` 可创建 `IReplayRuntime` 并持有 `RenderTexture`。

`renderProfile.mode` 首版取值：

- `canvas_2d`
- `scene_2d`
- `scene_3d`

`steps` 字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `index` | number | 是 | 步骤序号 |
| `command` | string | 是 | 给游戏逻辑执行的命令，首版只占位 |
| `notation` | string | 否 | 棋谱或用户可读文本 |
| `comment` | string | 否 | 解说文本 |
| `duration` | number | 否 | 单步播放时长，缺省走 `secondsPerStep` |

### section_title

栏目标题。

```json
{
  "id": "section_001",
  "type": "section_title",
  "text": "赛事快讯"
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `text` | string | 是 | 栏目标题 |

### news_item

资讯列表项。

```json
{
  "id": "news_001",
  "type": "news_item",
  "title": "象甲联赛第8轮：上海队3-1击败广东队",
  "source": "中国象棋协会",
  "time": "2小时前",
  "action": {
    "type": "open_article",
    "target": "article_detail",
    "params": {
      "articleId": "article_001"
    }
  }
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `title` | string | 是 | 资讯标题 |
| `source` | string | 否 | 来源 |
| `time` | string | 否 | 发布时间文本 |

### paragraph

文章正文段落。首版可先定义，文章详情阶段再实现。

```json
{
  "id": "paragraph_001",
  "type": "paragraph",
  "text": "屏风马破当头炮，午间挑战赛出现精彩杀局。"
}
```

### image

文章图片。图片资源由媒体服务异步加载，文章接口只下发 URL 或 `media` 引用。

```json
{
  "id": "image_001",
  "type": "image",
  "url": "https://cdn.example.com/articles/001/img1.webp",
  "aspectRatio": 1.6,
  "caption": "比赛现场",
  "media": {
    "mediaId": "article_001_img_001",
    "type": "image",
    "url": "https://cdn.example.com/articles/001/img1.webp",
    "mimeType": "image/webp",
    "version": "v1",
    "aspectRatio": 1.6
  }
}
```

### video

文章视频。首版渲染封面、标题、时长和播放占位，不接真实播放器。

```json
{
  "id": "video_001",
  "type": "video",
  "title": "关键回合复盘",
  "posterUrl": "https://cdn.example.com/articles/001/video_poster.webp",
  "streamUrl": "https://cdn.example.com/videos/001/master.m3u8",
  "aspectRatio": 1.777,
  "durationSeconds": 83,
  "media": {
    "mediaId": "article_001_video_001",
    "type": "video",
    "posterUrl": "https://cdn.example.com/articles/001/video_poster.webp",
    "streamUrl": "https://cdn.example.com/videos/001/master.m3u8",
    "mimeType": "application/vnd.apple.mpegurl",
    "version": "v1",
    "durationSeconds": 83
  }
}
```

### article_header

文章详情页头部。

```json
{
  "id": "article_title",
  "type": "article_header",
  "title": "象甲联赛第8轮：上海队3-1击败广东队",
  "source": "中国象棋协会",
  "time": "2小时前"
}
```

字段说明：

| 字段 | 类型 | 必填 | 说明 |
| --- | --- | --- | --- |
| `title` | string | 是 | 文章标题 |
| `source` | string | 否 | 来源 |
| `time` | string | 否 | 发布时间文本 |

### spacer

固定间距。

```json
{
  "id": "spacer_001",
  "type": "spacer",
  "height": 12
}
```

### divider

分割线。

```json
{
  "id": "divider_001",
  "type": "divider"
}
```

## 首页 mock 示例

```json
{
  "pageId": "home",
  "pageType": "home",
  "title": "今日棋坛",
  "blocks": [
    {
      "id": "featured_001",
      "type": "featured_match",
      "badge": "本日最佳对局",
      "title": "张三 15回合绝杀 李四",
      "subtitle": "屏风马破当头炮 · 午间挑战赛",
      "source": "中国象棋",
      "boardTitle": "终局局面",
      "fen": "mock-fen",
      "action": {
        "type": "open_match",
        "target": "match_detail",
        "params": {
          "matchId": "match_001"
        }
      }
    },
    {
      "id": "section_001",
      "type": "section_title",
      "text": "赛事快讯"
    },
    {
      "id": "news_001",
      "type": "news_item",
      "title": "象甲联赛第8轮：上海队3-1击败广东队",
      "source": "中国象棋协会",
      "time": "2小时前",
      "action": {
        "type": "open_article",
        "target": "article_detail",
        "params": {
          "articleId": "article_001"
        }
      }
    }
  ]
}
```

## 扩展规则

新增 block 时必须补充：

- block 类型名
- 字段说明
- JSON 示例
- 是否首页支持
- 是否详情页支持
- 对应 Unity View 名称
- 点击行为

新增 block 不应要求修改已有页面主体逻辑。
