using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public static class FeatureViewHelpers
    {
        public static Color ResultColor(string result)
        {
            switch (result)
            {
                case "胜":
                    return AppTheme.Rgb(91, 140, 62);
                case "负":
                    return AppTheme.Accent;
                default:
                    return AppTheme.SecondaryText;
            }
        }

        public static Image CreateCard(string name, Transform parent)
        {
            return AppUIFactory.CreateImage(name, parent, AppTheme.Surface);
        }

        public static Button CreateCardButton(string name, Transform parent, UnityEngine.Events.UnityAction onClick)
        {
            return AppUIFactory.CreateButton(name, parent, AppTheme.Surface, onClick);
        }

        public static TextMeshProUGUI CreateTitle(string name, Transform parent, string text, float size = 18f)
        {
            var label = AppUIFactory.CreateText(
                name,
                parent,
                text,
                size,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            label.maxVisibleLines = 1;
            return label;
        }

        public static TextMeshProUGUI CreateMeta(string name, Transform parent, string text, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var label = AppUIFactory.CreateText(
                name,
                parent,
                text,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                alignment);
            label.maxVisibleLines = 1;
            return label;
        }

        public static bool HasAction(FeatureItemData item)
        {
            return item != null
                && item.action != null
                && !string.IsNullOrEmpty(item.action.type)
                && item.action.type != "none";
        }
    }
}
