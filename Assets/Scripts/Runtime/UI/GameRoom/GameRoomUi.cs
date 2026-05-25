using NewsFramework.Data.GameRoom;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.GameRoom
{
    internal static class GameRoomUi
    {
        public static Image CreatePanel(string name, Transform parent, Color color)
        {
            var image = AppUIFactory.CreateImage(name, parent, color);
            image.raycastTarget = false;
            return image;
        }

        public static Button CreateTextButton(
            string name,
            Transform parent,
            string label,
            Color background,
            Color textColor,
            float fontSize,
            UnityEngine.Events.UnityAction onClick = null)
        {
            var button = AppUIFactory.CreateButton(name, parent, background, onClick);
            var text = AppUIFactory.CreateText(
                "Label",
                button.transform,
                label,
                fontSize,
                textColor,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(text.rectTransform);
            return button;
        }

        public static TextMeshProUGUI CreateLabel(
            string name,
            Transform parent,
            string text,
            float size,
            Color color,
            FontStyles style = FontStyles.Normal,
            TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var label = AppUIFactory.CreateText(name, parent, text, size, color, style, alignment);
            label.enableAutoSizing = false;
            return label;
        }

        public static Image CreateAvatar(string name, Transform parent, GamePlayerData player, float size)
        {
            var avatar = AppUIFactory.CreateImage(name, parent, GameRoomStyle.Alpha(GameRoomStyle.Gold, 0.18f));
            AppUIFactory.AddLayoutElement(avatar.gameObject, size, size);

            var label = CreateLabel(
                "AvatarText",
                avatar.transform,
                ResolveAvatarText(player),
                Mathf.Max(14f, size * 0.38f),
                GameRoomStyle.Gold,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);
            return avatar;
        }

        private static string ResolveAvatarText(GamePlayerData player)
        {
            if (player == null)
                return "\u68CB";
            if (!string.IsNullOrEmpty(player.avatarText))
                return player.avatarText;
            return string.IsNullOrEmpty(player.displayName) ? "\u68CB" : player.displayName.Substring(0, 1);
        }
    }
}