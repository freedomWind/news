using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;

namespace NewsFramework.Data.Mock
{
    public static class ProfileMockData
    {
        public static FeaturePageData Create()
        {
            return new FeaturePageData
            {
                pageId = "profile",
                title = "我的",
                sections = new List<FeatureSectionData>
                {
                    new FeatureSectionData
                    {
                        id = "profile_header",
                        type = "profile_header",
                        items = new List<FeatureItemData>
                        {
                            new FeatureItemData
                            {
                                id = "user_profile",
                                title = "张三",
                                badge = "棋坛名将",
                                icon = "张",
                                action = Action("open_profile", "user_profile")
                            }
                        }
                    },
                    new FeatureSectionData
                    {
                        id = "stats",
                        type = "stats_row",
                        items = new List<FeatureItemData>
                        {
                            Stat("total_matches", "总对局", "1283"),
                            Stat("win_rate", "胜率", "62%"),
                            Stat("streak", "连胜中", "7")
                        }
                    },
                    Header("achievement_header", "我的称号与成就", "查看全部 >",
                        Action("open_achievements", "achievements")),
                    new FeatureSectionData
                    {
                        id = "achievements",
                        type = "achievement_strip",
                        items = new List<FeatureItemData>
                        {
                            Achievement("wins_100", "百胜将军", "冠", false),
                            Achievement("famous", "棋坛名将", "谱", false),
                            Achievement("invincible", "独孤求败", "锁", true),
                            Achievement("master", "国手风范", "印", true)
                        }
                    },
                    Header("rank_header", "我的段位"),
                    new FeatureSectionData
                    {
                        id = "rank_list",
                        type = "rank_list",
                        items = new List<FeatureItemData>
                        {
                            Rank("xiangqi", "中国象棋", "天梯分：1850分", "业余五段", "車", "accent"),
                            Rank("go", "围棋", "天梯分：2100分", "业余六段", "弈", "dark")
                        }
                    },
                    Header("settings_header", "对局设置"),
                    new FeatureSectionData
                    {
                        id = "settings",
                        type = "settings_list",
                        items = new List<FeatureItemData>
                        {
                            Setting("board_style", "棋盘风格", "木质棋盘 · 深色", null),
                            Setting("move_sound", "走棋音效", "开启", "accent"),
                            Setting("default_time", "默认时限", "每步60秒", null)
                        }
                    },
                    new FeatureSectionData
                    {
                        id = "about_footer",
                        type = "about_footer",
                        title = "为棋友用心打造",
                        subtitle = "App版本 v1.0.0",
                        items = new List<FeatureItemData>
                        {
                            Setting("privacy", "隐私设置", string.Empty, null),
                            Setting("about", "关于我们", string.Empty, null)
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

        private static FeatureItemData Stat(string id, string title, string value)
        {
            return new FeatureItemData
            {
                id = id,
                title = title,
                value = value
            };
        }

        private static FeatureItemData Achievement(string id, string title, string icon, bool locked)
        {
            return new FeatureItemData
            {
                id = id,
                title = title,
                icon = icon,
                locked = locked,
                action = locked ? BlockActionData.None() : Action("open_achievement", id)
            };
        }

        private static FeatureItemData Rank(string id, string title, string subtitle, string value, string icon, string state)
        {
            return new FeatureItemData
            {
                id = id,
                title = title,
                subtitle = subtitle,
                value = value,
                icon = icon,
                state = state,
                action = Action("open_rank", id)
            };
        }

        private static FeatureItemData Setting(string id, string title, string value, string state)
        {
            return new FeatureItemData
            {
                id = id,
                title = title,
                value = value,
                state = state,
                action = Action("open_setting", id)
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
                    { "source", "profile_page" }
                }
            };
        }
    }
}
