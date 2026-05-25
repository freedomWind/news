using System;
using System.Collections.Generic;
using NewsFramework.Data.Features;
using NewsFramework.UI.Rendering;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public sealed class FeatureSectionRegistry
    {
        private readonly Dictionary<string, Func<Transform, FeatureSectionViewBase>> factories =
            new Dictionary<string, Func<Transform, FeatureSectionViewBase>>();
        private readonly RendererRegistry<PrefabRenderDescriptor> prefabRegistry =
            new RendererRegistry<PrefabRenderDescriptor>();

        public void Register(string type, Func<Transform, FeatureSectionViewBase> factory)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Feature section type cannot be empty.");
                return;
            }

            factories[type] = factory;
        }

        public void RegisterPrefab(string type, string prefabKey, string fallbackType = "")
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Feature section type cannot be empty.");
                return;
            }

            prefabRegistry.Register(type, new PrefabRenderDescriptor
            {
                type = type,
                prefabKey = prefabKey,
                fallbackType = fallbackType
            });
        }

        public FeatureSectionViewBase Create(FeatureSectionData data, Transform parent)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.type))
            {
                return UnknownFeatureSectionView.Create(parent, "empty");
            }

            if (ShouldUsePrefab(data, out var descriptor))
            {
                var fallback = ResolveFallbackFactory(data.type, descriptor);
                return PrefabFeatureSectionView.Create(parent, descriptor, fallback);
            }

            return factories.TryGetValue(data.type, out var factory)
                ? factory(parent)
                : UnknownFeatureSectionView.Create(parent, data.type);
        }

        public static FeatureSectionRegistry CreateDefault()
        {
            var registry = new FeatureSectionRegistry();
            registry.Register("section_header", FeatureSectionHeaderView.Create);
            registry.Register("quick_action_grid", QuickActionGridSectionView.Create);
            registry.Register("feature_action_card", FeatureActionCardSectionView.Create);
            registry.Register("recent_match_list", RecentMatchListSectionView.Create);
            registry.Register("profile_header", ProfileHeaderSectionView.Create);
            registry.Register("stats_row", StatsRowSectionView.Create);
            registry.Register("achievement_strip", AchievementStripSectionView.Create);
            registry.Register("rank_list", RankListSectionView.Create);
            registry.Register("settings_list", SettingsListSectionView.Create);
            registry.Register("about_footer", AboutFooterSectionView.Create);
            registry.Register("empty_state", EmptyStateSectionView.Create);
            registry.RegisterPrefab("settings_list_prefab", "Prefabs/Features/SettingsListSection", "settings_list");
            return registry;
        }

        private bool ShouldUsePrefab(FeatureSectionData data, out PrefabRenderDescriptor descriptor)
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
                    : new PrefabRenderDescriptor { type = data.type, fallbackType = data.fallbackType };
                return true;
            }

            return prefabRegistry.TryGet(data.type, out descriptor);
        }

        private Func<Transform, FeatureSectionViewBase> ResolveFallbackFactory(
            string type,
            PrefabRenderDescriptor descriptor)
        {
            if (descriptor != null && !string.IsNullOrWhiteSpace(descriptor.fallbackType) &&
                factories.TryGetValue(descriptor.fallbackType, out var fallbackFactory))
            {
                return fallbackFactory;
            }

            return !string.IsNullOrWhiteSpace(type) && factories.TryGetValue(type, out var factory)
                ? factory
                : null;
        }
    }
}
