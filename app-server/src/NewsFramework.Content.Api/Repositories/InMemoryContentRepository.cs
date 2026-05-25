using NewsFramework.Content.Api.Contracts;

namespace NewsFramework.Content.Api.Repositories;

public sealed class InMemoryContentRepository : IContentRepository
{
    private const string FeedVersion = "home_v1";
    private readonly List<PageData> articles;
    private readonly Dictionary<string, ArticleEngagementSummaryData> engagementByArticleId;

    public InMemoryContentRepository()
    {
        var coverMedia = new MediaAssetData
        {
            MediaId = "med_sample_cover",
            Type = "image",
            Url = "/api/media/variants/var_sample_cover/content",
            MimeType = "image/png",
            Version = "v1",
            Width = 1,
            Height = 1,
            AspectRatio = 1.0f
        };

        articles = new List<PageData>
        {
            new PageData
            {
                PageId = "article_001",
                PageType = "article",
                Title = "今日棋坛：开局选择与中盘转换",
                Version = "v1",
                ExpiresInSeconds = 86400,
                Blocks = new[]
                {
                    new BlockData
                    {
                        BlockId = "article_001_title",
                        BlockType = "title",
                        Title = "今日棋坛：开局选择与中盘转换",
                        ArticleId = "article_001"
                    },
                    new BlockData
                    {
                        BlockId = "article_001_cover",
                        BlockType = "image",
                        ArticleId = "article_001",
                        Media = coverMedia
                    },
                    new BlockData
                    {
                        BlockId = "article_001_p1",
                        BlockType = "paragraph",
                        ArticleId = "article_001",
                        Text = "本期样例文章用于验证 Unity 内容接口。正式数据接入后，这里会由内容仓储返回发布态正文块。"
                    }
                }
            },
            new PageData
            {
                PageId = "article_002",
                PageType = "article",
                Title = "实战复盘：优势局面的收束",
                Version = "v1",
                ExpiresInSeconds = 86400,
                Blocks = new[]
                {
                    new BlockData
                    {
                        BlockId = "article_002_title",
                        BlockType = "title",
                        Title = "实战复盘：优势局面的收束",
                        ArticleId = "article_002"
                    },
                    new BlockData
                    {
                        BlockId = "article_002_p1",
                        BlockType = "paragraph",
                        ArticleId = "article_002",
                        Text = "优势局面最容易出现节奏松动。样例内容先覆盖客户端文章详情渲染链路。"
                    }
                }
            },
            new PageData
            {
                PageId = "article_003",
                PageType = "article",
                Title = "训练计划：三步检查法",
                Version = "v1",
                ExpiresInSeconds = 86400,
                Blocks = new[]
                {
                    new BlockData
                    {
                        BlockId = "article_003_title",
                        BlockType = "title",
                        Title = "训练计划：三步检查法",
                        ArticleId = "article_003"
                    },
                    new BlockData
                    {
                        BlockId = "article_003_p1",
                        BlockType = "paragraph",
                        ArticleId = "article_003",
                        Text = "每手棋前先看威胁、候选和变化。后续可以扩展为结构化训练块。"
                    }
                }
            }
        };

        engagementByArticleId = new Dictionary<string, ArticleEngagementSummaryData>
        {
            ["article_001"] = new ArticleEngagementSummaryData
            {
                ArticleId = "article_001",
                Bookmarked = false,
                Flowered = false,
                CanComment = true,
                FlowerCount = 128,
                CommentCount = 8,
                PreviewComments = new[]
                {
                    new CommentPreviewData
                    {
                        CommentId = "comment_001",
                        AuthorId = "user_001",
                        AuthorName = "棋友A",
                        Text = "这个转换点很实用。",
                        CreatedAtUnixSeconds = 1779340000
                    }
                }
            },
            ["article_002"] = new ArticleEngagementSummaryData
            {
                ArticleId = "article_002",
                Bookmarked = false,
                Flowered = false,
                CanComment = true,
                FlowerCount = 76,
                CommentCount = 3
            },
            ["article_003"] = new ArticleEngagementSummaryData
            {
                ArticleId = "article_003",
                Bookmarked = false,
                Flowered = false,
                CanComment = true,
                FlowerCount = 42,
                CommentCount = 0
            }
        };
    }

    public Task<FeedPageResult> GetHomeFeedAsync(
        string cursor,
        int limit,
        string knownVersion,
        CancellationToken cancellationToken)
    {
        var normalizedLimit = Math.Clamp(limit <= 0 ? 10 : limit, 1, 50);
        var start = ParseCursor(cursor);
        var pageArticles = articles
            .Skip(start)
            .Take(normalizedLimit)
            .ToArray();

        var nextIndex = start + pageArticles.Length;
        var hasMore = nextIndex < articles.Count;

        var blocks = pageArticles.Select(article => new BlockData
        {
            BlockId = "feed_" + article.PageId,
            BlockType = "article_card",
            ArticleId = article.PageId,
            Title = article.Title,
            Text = FirstParagraph(article),
            ActionUrl = "/api/articles/" + article.PageId,
            Media = article.Blocks.FirstOrDefault(block => block.Media != null)?.Media,
            Metadata = new Dictionary<string, object>
            {
                ["version"] = article.Version
            }
        }).ToArray();

        var result = new FeedPageResult
        {
            FeedId = "home",
            Title = "今日棋坛",
            FeedVersion = FeedVersion,
            Cursor = cursor,
            NextCursor = hasMore ? nextIndex.ToString() : string.Empty,
            HasMore = hasMore,
            EstimatedTotal = articles.Count,
            ExpiresInSeconds = 120,
            Blocks = blocks
        };

        return Task.FromResult(result);
    }

    public Task<PageData?> GetArticleAsync(string articleId, string knownVersion, CancellationToken cancellationToken)
    {
        var article = articles.FirstOrDefault(item => item.PageId == articleId);
        return Task.FromResult(article);
    }

    public Task<ArticleEngagementSummaryData?> GetArticleEngagementSummaryAsync(
        string articleId,
        CancellationToken cancellationToken)
    {
        engagementByArticleId.TryGetValue(articleId, out var summary);
        return Task.FromResult(summary);
    }

    private static int ParseCursor(string cursor)
    {
        if (!int.TryParse(cursor, out var value))
        {
            return 0;
        }

        return Math.Max(0, value);
    }

    private static string FirstParagraph(PageData article)
    {
        return article.Blocks.FirstOrDefault(block => block.BlockType == "paragraph")?.Text ?? string.Empty;
    }
}
