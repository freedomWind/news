using System;
using System.Collections.Generic;
using NewsFramework.Data.Features;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public sealed class FeatureSectionRegistry
    {
        private readonly Dictionary<string, Func<Transform, FeatureSectionViewBase>> factories =
            new Dictionary<string, Func<Transform, FeatureSectionViewBase>>();

        public void Register(string type, Func<Transform, FeatureSectionViewBase> factory)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Feature section type cannot be empty.");
                return;
            }

            factories[type] = factory;
        }

        public FeatureSectionViewBase Create(FeatureSectionData data, Transform parent)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.type))
            {
                return UnknownFeatureSectionView.Create(parent, "empty");
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
            return registry;
        }
    }
}
