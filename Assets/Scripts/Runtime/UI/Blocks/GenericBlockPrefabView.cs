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
    public sealed class GenericBlockPrefabView : MonoBehaviour, IDataBoundView<BlockData>
    {
        [SerializeField] private string badgeBinding = "Badge";
        [SerializeField] private string titleBinding = "Title";
        [SerializeField] private string subtitleBinding = "Subtitle";
        [SerializeField] private string metaBinding = "Meta";
        [SerializeField] private bool rootButtonEmitsAction = true;

        private BlockData data;
        private Action<BlockActionData> onAction;
        private bool built;
        private TextMeshProUGUI badgeLabel;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;
        private TextMeshProUGUI metaLabel;

        public void Bind(BlockData blockData, Action<BlockActionData> actionHandler)
        {
            data = blockData;
            onAction = actionHandler;

            if (!built)
            {
                Build();
            }

            badgeLabel.text = string.IsNullOrEmpty(data?.badge) ? data?.type ?? "block" : data.badge;
            titleLabel.text = data?.title ?? data?.text ?? data?.boardTitle ?? string.Empty;
            subtitleLabel.text = data?.subtitle ?? data?.caption ?? string.Empty;
            metaLabel.text = BuildMeta(data);
        }

        private void Build()
        {
            built = true;
            if (GetComponent<RectTransform>() == null)
            {
                gameObject.AddComponent<RectTransform>();
            }

            var background = GetComponent<Image>();
            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
                background.color = AppTheme.Surface;
            }

            var button = GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }

            button.targetGraphic = background;
            button.onClick.RemoveListener(TriggerAction);
            if (rootButtonEmitsAction)
            {
                button.onClick.AddListener(TriggerAction);
            }

            if (GetComponent<LayoutElement>() == null)
            {
                AppUIFactory.AddLayoutElement(gameObject, 112f);
            }

            if (GetComponent<VerticalLayoutGroup>() == null)
            {
                AppUIFactory.AddVerticalLayout(gameObject, 7f, new RectOffset(14, 14, 12, 12));
            }

            badgeLabel = GetOrCreateLabel(badgeBinding, 12f, AppTheme.Accent, FontStyles.Bold);
            EnsureLayoutElement(badgeLabel.gameObject, 18f);

            titleLabel = GetOrCreateLabel(titleBinding, 18f, AppTheme.PrimaryText, FontStyles.Bold);
            titleLabel.maxVisibleLines = 2;
            EnsureLayoutElement(titleLabel.gameObject, 44f);

            subtitleLabel = GetOrCreateLabel(subtitleBinding, 13f, AppTheme.SecondaryText);
            subtitleLabel.maxVisibleLines = 2;
            EnsureLayoutElement(subtitleLabel.gameObject, 34f);

            metaLabel = GetOrCreateLabel(
                metaBinding,
                12f,
                AppTheme.TabInactive,
                FontStyles.Normal,
                TextAlignmentOptions.Right);
            EnsureLayoutElement(metaLabel.gameObject, 18f);
        }

        private void TriggerAction()
        {
            if (data == null || data.action == null || data.action.type == "none")
            {
                return;
            }

            onAction?.Invoke(data.action);
        }

        private static string BuildMeta(BlockData block)
        {
            if (block == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(block.source) && !string.IsNullOrEmpty(block.time))
            {
                return block.source + " | " + block.time;
            }

            if (!string.IsNullOrEmpty(block.source))
            {
                return block.source;
            }

            return block.time ?? string.Empty;
        }

        private TextMeshProUGUI GetOrCreateLabel(
            string labelName,
            float fontSize,
            Color color,
            FontStyles style = FontStyles.Normal,
            TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var child = transform.Find(labelName);
            if (child != null && child.TryGetComponent<TextMeshProUGUI>(out var existing))
            {
                return existing;
            }

            return AppUIFactory.CreateText(labelName, transform, string.Empty, fontSize, color, style, alignment);
        }

        private static void EnsureLayoutElement(GameObject target, float preferredHeight)
        {
            var layout = target.GetComponent<LayoutElement>();
            if (layout == null)
            {
                layout = target.AddComponent<LayoutElement>();
            }

            layout.preferredHeight = preferredHeight;
        }
    }
}
