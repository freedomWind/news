using NewsFramework.Data.Blocks;
using NewsFramework.Data.Replay;

namespace NewsFramework.Data.Mock
{
    public static class ArticleMockData
    {
        public static PageData Create(string articleId)
        {
            var page = new PageData
            {
                pageId = string.IsNullOrEmpty(articleId) ? "article_001" : articleId,
                pageType = "article",
                title = ResolveTitle(articleId)
            };

            page.blocks.Add(new BlockData
            {
                id = "article_title",
                type = "article_header",
                title = page.title,
                source = ResolveSource(articleId),
                time = ResolveTime(articleId)
            });

            page.blocks.Add(new BlockData
            {
                id = "paragraph_001",
                type = "paragraph",
                text = "本轮焦点战开局阶段节奏很快，红方以屏风马稳住中路，随后借助车马配合连续压迫黑方右翼。"
            });

            page.blocks.Add(new BlockData
            {
                id = "image_001",
                type = "image",
                url = "mock://match-photo",
                aspectRatio = 1.55f,
                caption = "比赛现场图占位"
            });

            page.blocks.Add(new BlockData
            {
                id = "paragraph_002",
                type = "paragraph",
                text = "第15回合成为整盘棋的转折点。红方弃子抢先，黑方虽然多得一兵，但主帅暴露，防线被连续牵制。"
            });

            page.blocks.Add(new BlockData
            {
                id = "replay_001",
                type = "replay",
                title = "关键回放",
                subtitle = "第12至14回合",
                replay = ReplayMockData.CreateArticlePreview()
            });

            page.blocks.Add(new BlockData
            {
                id = "board_001",
                type = "board_preview",
                title = "关键局面",
                boardTitle = "红方先手，三步入局",
                fen = "mock-article-fen",
                action = new BlockActionData
                {
                    type = "open_match",
                    target = "match_detail",
                    parameters =
                    {
                        { "matchId", "match_001" }
                    }
                }
            });

            page.blocks.Add(new BlockData
            {
                id = "paragraph_003",
                type = "paragraph",
                text = "这类局面对内容系统有代表性：普通段落、图片、棋盘预览和对局入口需要在同一个详情页里混排。"
            });

            return page;
        }

        private static string ResolveTitle(string articleId)
        {
            return articleId == "article_002"
                ? "王天一快棋赛连胜晋级，明日迎战许银川"
                : "象甲联赛第8轮：上海队3-1击败广东队";
        }

        private static string ResolveSource(string articleId)
        {
            return articleId == "article_002" ? "棋坛快讯" : "中国象棋协会";
        }

        private static string ResolveTime(string articleId)
        {
            return articleId == "article_002" ? "4小时前" : "2小时前";
        }
    }
}
