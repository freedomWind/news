using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class BlockRegistry
    {
        private readonly Dictionary<string, Func<Transform, BlockViewBase>> factories =
            new Dictionary<string, Func<Transform, BlockViewBase>>();

        public void Register(string type, Func<Transform, BlockViewBase> factory)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Block type cannot be empty.");
                return;
            }

            factories[type] = factory;
        }

        public BlockViewBase Create(BlockData data, Transform parent)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.type))
            {
                return CreateUnknown(parent, "empty");
            }

            if (!factories.TryGetValue(data.type, out var factory))
            {
                return CreateUnknown(parent, data.type);
            }

            return factory(parent);
        }

        public static BlockRegistry CreateDefault()
        {
            var registry = new BlockRegistry();
            registry.Register("featured_match", FeaturedMatchBlockView.Create);
            registry.Register("board_preview", BoardPreviewBlockView.Create);
            registry.Register("section_title", SectionTitleBlockView.Create);
            registry.Register("news_item", NewsItemBlockView.Create);
            registry.Register("article_card", NewsItemBlockView.Create);
            registry.Register("live_match_item", LiveMatchItemBlockView.Create);
            registry.Register("article_tip_card", ArticleTipCardBlockView.Create);
            registry.Register("lifestyle_card", LifestyleCardBlockView.Create);
            registry.Register("spacer", SpacerBlockView.Create);
            registry.Register("divider", DividerBlockView.Create);
            registry.Register("paragraph", ParagraphBlockView.Create);
            registry.Register("image", ImageBlockView.Create);
            registry.Register("video", VideoBlockView.Create);
            registry.Register("replay_preview", ReplayPreviewBlockView.Create);
            registry.Register("replay", ReplayBlockView.Create);
            registry.Register("article_header", ArticleHeaderBlockView.Create);
            registry.Register("title", ArticleHeaderBlockView.Create);
            return registry;
        }

        private static BlockViewBase CreateUnknown(Transform parent, string type)
        {
            var view = UnknownBlockView.Create(parent);
            view.SetUnknownType(type);
            return view;
        }
    }
}
