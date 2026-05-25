using System.Collections.Generic;
using System.Linq;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Mock;
using NewsFramework.Data.Replay;

namespace NewsFramework.Editor.PrefabScaffolder
{
    internal static class BlockSampleCatalog
    {
        public static readonly string[] SampleSources =
        {
            "Built-in mock data",
            "Synthetic scaffold sample"
        };

        public static readonly string[] BlockTypes =
        {
            "article_header",
            "article_tip_card",
            "board_preview",
            "divider",
            "featured_match",
            "game_entry_card",
            "image",
            "lifestyle_card",
            "live_match_item",
            "news_item",
            "paragraph",
            "replay",
            "replay_preview",
            "section_title",
            "spacer",
            "video"
        };

        public static BlockData CreateSample(string type, int sampleSourceIndex)
        {
            if (sampleSourceIndex == 0)
            {
                var homeSample = HomeMockData.Create().blocks.FirstOrDefault(block => block.type == type);
                if (homeSample != null)
                {
                    return CloneForScaffold(homeSample);
                }
            }

            return CreateSyntheticSample(type);
        }

        private static BlockData CreateSyntheticSample(string type)
        {
            return type switch
            {
                "game_entry_card" => new BlockData
                {
                    id = "scaffold_game_entry",
                    type = "game_entry_card",
                    badge = "Prefab",
                    title = "Player A vs Player B",
                    subtitle = "Round 12 | 23 viewers",
                    source = "Prefab scaffold sample",
                    action = new BlockActionData { type = "open_match", target = "match_live" }
                },
                "article_header" => new BlockData
                {
                    id = "scaffold_article_header",
                    type = "article_header",
                    title = "2026 Championship recap",
                    subtitle = "32 players in Hangzhou",
                    source = "News demo",
                    time = "2026-05-20"
                },
                "paragraph" => new BlockData
                {
                    id = "scaffold_paragraph",
                    type = "paragraph",
                    text = "This is scaffold sample text for a data-bound article paragraph."
                },
                "replay" => new BlockData
                {
                    id = "scaffold_replay",
                    type = "replay",
                    title = "Replay scaffold",
                    replay = ReplayMockData.CreateArticlePreview()
                },
                _ => new BlockData
                {
                    id = "scaffold_" + type,
                    type = type,
                    title = type + " scaffold",
                    subtitle = "Sample data generated in Editor.",
                    action = new BlockActionData { type = "preview_action", target = type }
                }
            };
        }

        private static BlockData CloneForScaffold(BlockData source)
        {
            return new BlockData
            {
                id = "scaffold_" + source.id,
                type = source.type,
                rendererKey = string.Empty,
                prefabKey = string.Empty,
                fallbackType = string.Empty,
                marginTop = source.marginTop,
                marginBottom = source.marginBottom,
                action = CloneAction(source.action),
                badge = source.badge,
                title = source.title,
                subtitle = source.subtitle,
                source = source.source,
                boardTitle = source.boardTitle,
                fen = source.fen,
                text = source.text,
                time = source.time,
                url = source.url,
                posterUrl = source.posterUrl,
                streamUrl = source.streamUrl,
                caption = source.caption,
                aspectRatio = source.aspectRatio,
                height = source.height,
                durationSeconds = source.durationSeconds,
                media = source.media,
                replay = source.replay
            };
        }

        private static BlockActionData CloneAction(BlockActionData source)
        {
            if (source == null)
            {
                return BlockActionData.None();
            }

            var action = new BlockActionData
            {
                type = source.type,
                target = source.target,
                parameters = new Dictionary<string, string>()
            };

            if (source.parameters != null)
            {
                foreach (var pair in source.parameters)
                {
                    action.parameters[pair.Key] = pair.Value;
                }
            }

            return action;
        }
    }
}
