using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.UI.Rendering;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class BlockRegistry
    {
        private readonly Dictionary<string, Func<Transform, BlockViewBase>> factories =
            new Dictionary<string, Func<Transform, BlockViewBase>>();
        private readonly RendererRegistry<PrefabRenderDescriptor> prefabRegistry =
            new RendererRegistry<PrefabRenderDescriptor>();

        public void Register(string type, Func<Transform, BlockViewBase> factory)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Block type cannot be empty.");
                return;
            }

            factories[type] = factory;
        }

        public void RegisterPrefab(string type, string prefabKey, string fallbackType = "")
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Block type cannot be empty.");
                return;
            }

            prefabRegistry.Register(type, new PrefabRenderDescriptor
            {
                type = type,
                prefabKey = prefabKey,
                fallbackType = fallbackType
            });
        }

        public BlockViewBase Create(BlockData data, Transform parent)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.type))
            {
                return CreateUnknown(parent, "empty");
            }

            if (ShouldUsePrefab(data, out var descriptor))
            {
                return PrefabBlockView.Create(parent, descriptor);
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

        private bool ShouldUsePrefab(BlockData data, out PrefabRenderDescriptor descriptor)
        {
            descriptor = null;
            if (data == null)
            {
                return false;
            }

            if (string.Equals(data.rendererKey, "prefab", StringComparison.OrdinalIgnoreCase) ||
                !string.IsNullOrWhiteSpace(data.prefabKey))
            {
                if (!string.IsNullOrWhiteSpace(data.prefabKey))
                {
                    descriptor = new PrefabRenderDescriptor
                    {
                        type = data.type,
                        prefabKey = data.prefabKey,
                        fallbackType = data.fallbackType
                    };
                    return true;
                }

                descriptor = prefabRegistry.TryGet(data.type, out var registered)
                    ? registered
                    : new PrefabRenderDescriptor { type = data.type };
                return true;
            }

            return prefabRegistry.TryGet(data.type, out descriptor);
        }

        private static BlockViewBase CreateUnknown(Transform parent, string type)
        {
            var view = UnknownBlockView.Create(parent);
            view.SetUnknownType(type);
            return view;
        }
    }
}
