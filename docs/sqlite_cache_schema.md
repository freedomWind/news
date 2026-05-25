# SQLite 缓存设计

## 目标

SQLite 只负责本地结构化缓存，不承担媒体二进制存储。

当前缓存分两类：

- `content_pages`：文章、回放、普通详情页。
- `feed_pages`：首页、频道、搜索等列表流的 cursor 分页片段。

## 工具层

SQLite 底层封装来自 sample 中的 `SQLite4Unity3d`：

```text
Assets/App/Runtime/Services/Persistence/Sqlite/SQLite4Unity3d/SQLite.cs
```

项目内部不直接把这套 API 暴露给内容服务，而是通过：

```text
ISqliteDatabase
SqliteDatabase
SqliteParameter
SqliteRow
```

这样后续即使替换 SQLite 封装，内容服务仓库也不需要整体重写。

## content_pages

用于按需详情内容：

- `page_id`：详情页 id，主键。
- `page_type`：`page/article/replay`。
- `content_kind`：业务内容类型。
- `version`：服务端内容版本。
- `json`：完整 `PageData` JSON。
- `fetched_at`：拉取时间。
- `expires_at`：过期时间。
- `source`：来源。
- `updated_at`：本地更新时间。

## feed_pages

用于分页 feed：

- `feed_id`：feed 标识，例如 `home`。
- `feed_version`：服务端 feed 版本。
- `cursor`：当前页 cursor，第一页为空字符串。
- `next_cursor`：下一页 cursor。
- `title`：feed 标题。
- `blocks_json`：本页 block JSON 数组。
- `has_more`：是否还有下一页。
- `estimated_total`：服务端估算总量。
- `fetched_at`：拉取时间。
- `expires_at`：过期时间。
- `source`：来源。
- `updated_at`：本地更新时间。

`feed_pages` 主键为 `(feed_id, cursor)`。

## 访问规则

- 首页列表走 `IFeedPager`，不走 `IContentService`。
- 文章/回放/普通详情页走 `IContentService`。
- 远程结果写回 SQLite。
- 第一页 feed 即使命中缓存，也会后台请求远程，用于接收服务器最新首页列表。
- 后续页优先使用可用缓存，过期后再请求远程。

## Runtime Mode

`HomeDemoBootstrap` 通过 `ContentRuntimeConfig` 选择内容服务组合：

- `Mock`：默认模式，feed 和详情都走 mock。
- `Http`：feed 和详情都走 HTTP，不落 SQLite。
- `SQLiteMock`：feed 和详情走 mock 远端，同时将结果落到 SQLite。
- `SQLiteHttp`：feed 和详情走 HTTP，同时将结果落到 SQLite。

`SQLiteMock` 和 `SQLiteHttp` 下详情内容和 feed 分页共用同一个数据库连接。

## Cache Maintenance

`SQLiteMock` 和 `SQLiteHttp` 启动后默认执行一次轻量清理。

默认策略：

- 删除 `expires_at` 已过期的 `content_pages`。
- 删除 `expires_at` 已过期的 `feed_pages`。
- 删除同一 `feed_id` 下旧 `feed_version` 的分页。
- 详情页最多保留最近 200 条。
- 每个 feed 最多保留最近 30 页。

Unity Editor 菜单：

```text
NewsFramework/Diagnostics/Print SQLite Cache Stats
NewsFramework/Diagnostics/Cleanup SQLite Cache
```

## Smoke Test

Unity Editor 菜单：

```text
NewsFramework/Diagnostics/Run SQLite Smoke Test
```

该测试会在项目 `Temp/SqliteSmoke/` 下创建临时数据库，并验证：

- 原始 SQL 建表、写入和读取。
- `SQLiteContentLocalRepository` 保存和读取详情页。
- `SQLiteFeedPageLocalRepository` 保存和读取 feed 分页。
- `SqliteContentCacheMaintenance` 删除过期缓存。
