# 内容 HTTP 协议

## 目标

内容接口只负责下发结构化内容和缓存元信息，不直接暴露 Unity UI 细节。

客户端内部有两条独立链路：

```text
首页/频道信息流：
FeedPageRequest -> IFeedPageRemoteRepository -> FeedPageResult -> BlockRenderer

文章/回放/普通页面详情：
ContentRequest -> IContentRemoteRepository -> ContentCacheEntry -> PageData -> BlockRenderer
```

HTTP 分别是 `IFeedPageRemoteRepository` 和 `IContentRemoteRepository` 的一种实现。

## 接口

首版约定：

```text
GET /api/feed/{feedId}
GET /api/articles/{articleId}
GET /api/articles/{articleId}/engagement-summary
GET /api/content/replay/{pageId}
GET /api/content/page/{pageId}
```

详情接口可附加 query：

```text
contentKind=article|replay|page
pageType=article|replay|page
knownVersion=xxx
```

`ContentRequest.parameters` 中的键值会继续拼到详情 query，用于灰度、语言、模拟版本等扩展参数。

首页 feed 支持 cursor 分页：

```text
GET /api/feed/home?cursor=&limit=12&knownVersion=home_v1
```

`FeedPageRequest.parameters` 中的键值会继续拼到 feed query，用于频道、推荐策略、灰度和语言等扩展参数。

分页响应应包含：

```json
{
  "success": true,
  "message": "",
  "serverTime": 1779264000,
  "data": {
    "feedId": "home",
    "title": "今日棋坛",
    "feedVersion": "home_20260520_001",
    "cursor": "",
    "nextCursor": "cursor_abc",
    "hasMore": true,
    "estimatedTotal": 120,
    "expiresInSeconds": 7200,
    "blocks": []
  }
}
```

客户端优先使用 `cursor/nextCursor/hasMore`，`estimatedTotal` 只作为辅助展示或调试信息，不作为分页正确性的唯一依据。

## 响应结构

```json
{
  "success": true,
  "message": "",
  "serverTime": 1779264000,
  "data": {
    "pageId": "home",
    "pageType": "home",
    "title": "今日棋坛",
    "version": "home_20260520_001",
    "expiresInSeconds": 7200,
    "blocks": []
  }
}
```

字段说明：

- `success`：`true` 表示成功。客户端兼容旧的 `code = 0` 响应。
- `code`：旧协议成功码，`0` 表示成功；新服务端优先使用 `success`。
- `message`：错误或提示信息。
- `serverTime`：服务端 Unix 时间，暂不参与客户端逻辑。
- `data.version`：内容版本，客户端用它判断是否需要刷新 UI。
- `data.expiresInSeconds`：缓存 TTL；缺省时客户端使用本地默认策略。
- `data.blocks`：block schema 内容块列表。

## Block 示例

当前客户端使用 `Newtonsoft.Json` 反序列化，action 参数可以直接使用对象形式。

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
    "parameters": {
      "articleId": "article_001",
      "knownVersion": "article_001_v3"
    }
  }
}
```

服务端可以下发客户端暂时不识别的额外字段；当前 DTO mapper 会忽略未知字段，后续新增 block 字段时再显式映射。

## 文章互动摘要

文章正文和评论/收藏/献花等互动状态走独立接口，避免把业务交互状态塞进 `BlockSchema`。

```text
GET /api/articles/{articleId}/engagement-summary
```

首版响应可以直接返回摘要对象：

```json
{
  "articleId": "article_001",
  "bookmarked": false,
  "flowered": false,
  "canComment": true,
  "flowerCount": 128,
  "commentCount": 8,
  "previewComments": [
    {
      "commentId": "comment_001",
      "authorName": "老棋迷张伯",
      "avatarText": "张",
      "time": "2小时前",
      "text": "柳老的棋确实有电脑风范，弃子果断。",
      "authorReply": {
        "authorName": "作者回复",
        "text": "感谢支持。"
      }
    }
  ]
}
```

客户端也兼容包一层的响应：

```json
{
  "success": true,
  "data": {
    "articleId": "article_001",
    "bookmarked": false,
    "flowered": false,
    "canComment": true,
    "flowerCount": 128,
    "commentCount": 8,
    "previewComments": []
  }
}
```

评论预览只用于详情页首屏尾部展示；完整评论列表分页、评论发布、收藏/献花写入接口后续单独定义。

## 客户端实现

当前已新增：

- `IContentHttpClient`
- `UnityWebRequestContentHttpClient`
- `ContentHttpConfig`
- `ContentHttpErrorClassifier`
- `RetryingContentHttpClient`
- `IContentJsonSerializer`
- `NewtonsoftContentJsonSerializer`
- `ContentServerEnvelopeDto`
- `ContentServerDtoMapper`
- `HttpContentRemoteRepository`
- `ArticleEngagementEnvelopeDto`
- `HttpArticleEngagementRemoteRepository`
- `RemoteArticleEngagementService`

默认 demo 仍使用 mock 远端仓库。要切到 HTTP，需要用：

```csharp
var config = new ContentHttpConfig
{
    baseUrl = "http://localhost:5235",
    maxRetryCount = 1
};

var service = ContentServiceFactory.CreateHttpCachedService(config);
```

Unity 编辑器中可通过菜单生成本地联调场景：

```text
NewsFramework/Build Content.Api Smoke Scene
```

生成的 `Assets/App/Scenes/HomeDemoContentApi.unity` 默认使用：

```text
Content.Api: http://localhost:5235
Media.Api:   http://localhost:5234
```

## 约束

- 服务端必须稳定下发 `version`。
- 首页 feed 只下发列表摘要和跳转信息，不下发完整文章正文。
- 文章详情在用户点击后按需请求。
- 文章互动摘要在文章详情打开后独立请求，首版一次返回收藏状态、献花状态、计数和评论预览。
- 回放 payload 可以独立接口请求，避免首页数据过重。
- 旧客户端遇到未知 block 类型必须能降级显示或忽略。

## 媒体分离

文章接口只返回文本、block 顺序和媒体引用，不返回图片或视频二进制。

图片 block 示例：

```json
{
  "id": "image_001",
  "type": "image",
  "caption": "比赛现场",
  "aspectRatio": 1.6,
  "media": {
    "mediaId": "article_001_img_001",
    "type": "image",
    "url": "https://cdn.example.com/articles/001/img1.webp",
    "mimeType": "image/webp",
    "version": "v1"
  }
}
```

视频 block 示例：

```json
{
  "id": "video_001",
  "type": "video",
  "title": "关键回合复盘",
  "media": {
    "mediaId": "article_001_video_001",
    "type": "video",
    "posterUrl": "https://cdn.example.com/articles/001/poster.webp",
    "streamUrl": "https://cdn.example.com/videos/001/master.m3u8",
    "mimeType": "application/vnd.apple.mpegurl",
    "durationSeconds": 83
  }
}
```

客户端缓存也分离：

```text
ContentCache：缓存文章 JSON、block 顺序、version、TTL
MediaCache：缓存图片纹理、视频封面、后续的视频分片或播放器缓存索引
```
