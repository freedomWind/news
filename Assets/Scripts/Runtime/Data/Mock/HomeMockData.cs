using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Media;
using NewsFramework.Data.Replay;

namespace NewsFramework.Data.Mock
{
    public static class HomeMockData
    {
        public static PageData Create()
        {
            var page = CreatePage();
            page.blocks.AddRange(CreateFeedBlocks());
            return page;
        }

        public static PageData CreatePaged(int offset, int limit, out int totalBlocks)
        {
            var allBlocks = CreateFeedBlocks();
            totalBlocks = allBlocks.Count;

            var page = CreatePage();
            if (limit <= 0 || offset >= totalBlocks)
            {
                return page;
            }

            offset = offset < 0 ? 0 : offset;
            var count = limit;
            if (offset + count > totalBlocks)
            {
                count = totalBlocks - offset;
            }

            page.blocks.AddRange(allBlocks.GetRange(offset, count));
            return page;
        }

        private static PageData CreatePage()
        {
            return new PageData
            {
                pageId = "home",
                pageType = "home",
                title = "今日棋坛"
            };
        }

        private static List<BlockData> CreateFeedBlocks()
        {
            var blocks = new List<BlockData>();

            blocks.Add(new BlockData
            {
                id = "featured_001",
                type = "featured_match",
                badge = "本日最佳对局",
                title = "张三 15回合绝杀 李四",
                subtitle = "屏风马破当头炮 · 午间挑战赛",
                source = "中国象棋",
                boardTitle = "终局局面",
                fen = "mock-fen",
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

            blocks.Add(new BlockData
            {
                id = "section_001",
                type = "section_title",
                text = "此刻棋坛",
                subtitle = "（有2局围观中）"
            });

            blocks.Add(new BlockData
            {
                id = "live_match_001",
                type = "live_match_item",
                badge = "♞",
                title = "赵六 vs 王五",
                subtitle = "第42回合 · 23人围观",
                action = new BlockActionData
                {
                    type = "open_match",
                    target = "match_live",
                    parameters =
                    {
                        { "matchId", "match_live_001" }
                    }
                }
            });

            blocks.Add(new BlockData
            {
                id = "live_match_002",
                type = "live_match_item",
                badge = "♜",
                title = "孙七 vs 周八",
                subtitle = "第15回合 · 12人围观",
                action = new BlockActionData
                {
                    type = "open_match",
                    target = "match_live",
                    parameters =
                    {
                        { "matchId", "match_live_002" }
                    }
                }
            });

            blocks.Add(new BlockData
            {
                id = "section_002",
                type = "section_title",
                text = "赛事快讯"
            });

            blocks.Add(CreateNewsItem(
                "news_001",
                "象甲联赛第8轮：上海队3-1击败广东队",
                "中国象棋协会",
                "2小时前",
                "article_001"));

            blocks.Add(CreateNewsItem(
                "news_002",
                "2025年全国象棋个人赛预选赛名单公布",
                "棋牌管理中心",
                "5小时前",
                "article_002"));

            blocks.Add(CreateNewsItem(
                "news_003",
                "第五届“楚河汉界”世界棋王赛门票开售",
                "赛事组委会",
                "1天前",
                "article_003"));

            blocks.Add(new BlockData
            {
                id = "section_003",
                type = "section_title",
                text = "棋艺干货"
            });

            blocks.Add(CreateArticleTip(
                "tip_001",
                "如何破解顺手炮布局陷阱",
                "针对你最近的战绩分析，你在面对顺手炮时胜率仅32%，本文详解三种常见陷阱及破解方法",
                "article_001"));

            blocks.Add(CreateArticleTip(
                "tip_002",
                "残局定式：车兵对士象全的必胜要诀",
                "掌握这五个关键步骤，车兵方可以在残局中稳操胜券",
                "article_002"));

            blocks.Add(new BlockData
            {
                id = "section_004",
                type = "section_title",
                text = "棋味生活"
            });

            blocks.Add(new BlockData
            {
                id = "life_001",
                type = "lifestyle_card",
                title = "棋友茶话会：每周三线下对弈活动",
                text = "本周三下午2点，老茶馆二楼，欢迎棋友自带棋具",
                time = "每周三 · 线下活动",
                url = "mock://chess-tea-life",
                media = new MediaAssetData
                {
                    mediaId = "life_001_thumb",
                    type = "image",
                    url = "mock://chess-tea-life",
                    mimeType = "image/mock",
                    version = "v1",
                    width = 480,
                    height = 480,
                    aspectRatio = 1f
                },
                action = new BlockActionData
                {
                    type = "open_article",
                    target = "article_detail",
                    parameters =
                    {
                        { "articleId", "article_003" }
                    }
                }
            });
            return blocks;
        }

        private static BlockData CreateArticleTip(
            string id,
            string title,
            string text,
            string articleId)
        {
            return new BlockData
            {
                id = id,
                type = "article_tip_card",
                title = title,
                text = text,
                badge = "AI生成",
                action = new BlockActionData
                {
                    type = "open_article",
                    target = "article_detail",
                    parameters =
                    {
                        { "articleId", articleId }
                    }
                }
            };
        }

        private static BlockData CreateNewsItem(
            string id,
            string title,
            string source,
            string time,
            string articleId,
            string imageUrl = "")
        {
            var block = new BlockData
            {
                id = id,
                type = "news_item",
                title = title,
                source = source,
                time = time
            };

            if (!string.IsNullOrEmpty(articleId))
            {
                block.action = new BlockActionData
                {
                    type = "open_article",
                    target = "article_detail",
                    parameters =
                    {
                        { "articleId", articleId }
                    }
                };
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                block.url = imageUrl;
                block.aspectRatio = 1.33f;
                block.media = new MediaAssetData
                {
                    mediaId = id + "_thumb",
                    type = "image",
                    url = imageUrl,
                    mimeType = "image/mock",
                    version = "v1",
                    width = 480,
                    height = 360,
                    aspectRatio = 1.33f
                };
            }

            return block;
        }
    }
}
