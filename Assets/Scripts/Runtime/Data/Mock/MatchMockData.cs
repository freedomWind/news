using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;

namespace NewsFramework.Data.Mock
{
    public static class MatchMockData
    {
        public static FeaturePageData Create()
        {
            return new FeaturePageData
            {
                pageId = "match",
                title = "对局",
                sections = new List<FeatureSectionData>
                {
                    new FeatureSectionData
                    {
                        id = "quick_start",
                        type = "quick_action_grid",
                        items = new List<FeatureItemData>
                        {
                            Item("daily_puzzle", "残局挑战", "每天一局 · 破解经典", "今日：七星聚会", "棋",
                                Action("open_puzzle", "daily_puzzle")),
                            Item("ai_practice", "AI陪练", "9级难度 · 随时对弈", "继续上次段位", "AI",
                                Action("open_ai_practice", "ai_practice"))
                        }
                    },
                    Header("human_header", "与人对弈"),
                    ActionCard(
                        "ranked_match",
                        "业余五段",
                        "天梯分 1850",
                        "在线",
                        "开始匹配",
                        Action("start_ranked_match", "ranked_match")),
                    ActionCard(
                        "friend_room",
                        "好友约战",
                        "创建房间 · 微信邀请",
                        string.Empty,
                        "创建房间",
                        Action("create_friend_room", "friend_room")),
                    Header("recent_header", "最近对局", "查看全部 >",
                        Action("open_match_history", "match_history")),
                    new FeatureSectionData
                    {
                        id = "recent_matches",
                        type = "recent_match_list",
                        items = new List<FeatureItemData>
                        {
                            Match("match_001", "张三 vs 李四", "胜", "2分钟前"),
                            Match("match_002", "王老五 vs 赵六", "负", "1小时前"),
                            Match("match_003", "陈大爷 vs 周老板", "和", "昨天"),
                            Match("match_004", "老李 vs 小王", "胜", "5月18日")
                        }
                    }
                }
            };
        }

        private static FeatureSectionData Header(string id, string title, string actionText = null, BlockActionData action = null)
        {
            return new FeatureSectionData
            {
                id = id,
                type = "section_header",
                title = title,
                actionText = actionText ?? string.Empty,
                action = action ?? BlockActionData.None()
            };
        }

        private static FeatureSectionData ActionCard(
            string id,
            string title,
            string subtitle,
            string value,
            string actionText,
            BlockActionData action)
        {
            return new FeatureSectionData
            {
                id = id + "_section",
                type = "feature_action_card",
                actionText = actionText,
                items = new List<FeatureItemData>
                {
                    new FeatureItemData
                    {
                        id = id,
                        title = title,
                        subtitle = subtitle,
                        value = value,
                        action = action
                    }
                }
            };
        }

        private static FeatureItemData Match(string id, string title, string result, string time)
        {
            return new FeatureItemData
            {
                id = id,
                title = title,
                result = result,
                time = time,
                action = Action("open_match_record", id)
            };
        }

        private static FeatureItemData Item(
            string id,
            string title,
            string subtitle,
            string badge,
            string icon,
            BlockActionData action)
        {
            return new FeatureItemData
            {
                id = id,
                title = title,
                subtitle = subtitle,
                badge = badge,
                icon = icon,
                action = action
            };
        }

        private static BlockActionData Action(string type, string target)
        {
            return new BlockActionData
            {
                type = type,
                target = target,
                parameters =
                {
                    { "source", "match_page" }
                }
            };
        }
    }
}
