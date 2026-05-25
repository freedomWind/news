using System;
using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Rendering;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class GameEntryPrefabBlockView : MonoBehaviour, IDataBoundView<BlockData>
    {
        private BlockData data;
        private Action<BlockActionData> onAction;
        private bool built;
        private TextMeshProUGUI badgeLabel;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;
        private TextMeshProUGUI sourceLabel;

        public void Bind(BlockData blockData, Action<BlockActionData> actionHandler)
        {
            data = blockData;
            onAction = actionHandler;

            if (!built)
            {
                Build();
            }

            badgeLabel.text = string.IsNullOrEmpty(data.badge) ? "Prefab" : data.badge;
            titleLabel.text = data.title ?? string.Empty;
            subtitleLabel.text = data.subtitle ?? data.text ?? string.Empty;
            sourceLabel.text = string.IsNullOrEmpty(data.source) ? "Prefab renderer entry" : data.source;
        }

        private void Build()
        {
            built = true;
            var rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = gameObject.AddComponent<RectTransform>();
            }

            AppUIFactory.AddLayoutElement(gameObject, 132f);
            var background = gameObject.GetComponent<Image>();
            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
            }

            background.color = AppTheme.Surface;
            var button = gameObject.GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }

            button.targetGraphic = background;
            button.onClick.RemoveListener(TriggerAction);
            button.onClick.AddListener(TriggerAction);

            var layout = gameObject.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = AppUIFactory.AddVerticalLayout(gameObject, 8f, new RectOffset(16, 16, 14, 14));
            }

            layout.spacing = 8f;
            layout.padding = new RectOffset(16, 16, 14, 14);
            badgeLabel = AppUIFactory.CreateText(
                "PrefabBadge",
                transform,
                string.Empty,
                12f,
                AppTheme.Accent,
                FontStyles.Bold);
            AppUIFactory.AddLayoutElement(badgeLabel.gameObject, 20f);

            titleLabel = AppUIFactory.CreateText(
                "PrefabTitle",
                transform,
                string.Empty,
                20f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            titleLabel.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(titleLabel.gameObject, 28f);

            subtitleLabel = AppUIFactory.CreateText(
                "PrefabSubtitle",
                transform,
                string.Empty,
                14f,
                AppTheme.SecondaryText);
            subtitleLabel.maxVisibleLines = 2;
            AppUIFactory.AddLayoutElement(subtitleLabel.gameObject, 40f);

            sourceLabel = AppUIFactory.CreateText(
                "PrefabSource",
                transform,
                string.Empty,
                12f,
                AppTheme.TabInactive,
                FontStyles.Normal,
                TextAlignmentOptions.Right);
            AppUIFactory.AddLayoutElement(sourceLabel.gameObject, 18f);
        }

        private void TriggerAction()
        {
            if (data == null || data.action == null || data.action.type == "none")
            {
                return;
            }

            onAction?.Invoke(data.action);
        }
    }
}
