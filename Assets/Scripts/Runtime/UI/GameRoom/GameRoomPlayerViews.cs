using NewsFramework.Data.GameRoom;
using NewsFramework.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.GameRoom
{
    internal static class GameRoomPlayerViews
    {
        public static void BuildTrainingOpponentPanel(Transform parent, GameRoomData data)
        {
            var panel = GameRoomUi.CreatePanel("OpponentPanel", parent, GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0.95f));
            AppUIFactory.AddLayoutElement(panel.gameObject, 68f);
            AppUIFactory.AddHorizontalLayout(panel.gameObject, 12f, new RectOffset(16, 16, 8, 8), TextAnchor.MiddleLeft);

            BuildIdentity(panel.transform, data != null ? data.blackPlayer : null, false, 1f);
            BuildRoundBadge(panel.transform, data != null ? data.roundText : string.Empty, 94f);
            BuildTimer(panel.transform, data != null && data.blackPlayer != null ? data.blackPlayer.timerText : string.Empty, false);
        }

        public static void BuildTrainingPlayerPanel(Transform parent, GameRoomData data)
        {
            var panel = GameRoomUi.CreatePanel("LocalPlayerPanel", parent, GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0.95f));
            AppUIFactory.AddLayoutElement(panel.gameObject, 76f);
            AppUIFactory.AddHorizontalLayout(panel.gameObject, 12f, new RectOffset(16, 16, 10, 10), TextAnchor.MiddleLeft);

            BuildIdentity(panel.transform, data != null ? data.redPlayer : null, true, 1f);
            BuildTimer(panel.transform, data != null && data.redPlayer != null ? data.redPlayer.timerText : string.Empty, true);
        }

        public static void BuildSpectatorVersusBar(Transform parent, GameRoomData data)
        {
            var panel = GameRoomUi.CreatePanel("SpectatorVersusBar", parent, GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0.92f));
            AppUIFactory.AddLayoutElement(panel.gameObject, 72f);
            AppUIFactory.AddHorizontalLayout(panel.gameObject, 8f, new RectOffset(10, 10, 8, 8), TextAnchor.MiddleLeft);

            BuildIdentity(panel.transform, data != null ? data.redPlayer : null, true, 1f);
            BuildSpectatorCenter(panel.transform, data);
            BuildIdentity(panel.transform, data != null ? data.blackPlayer : null, false, 1f, true);
        }

        private static void BuildIdentity(
            Transform parent,
            GamePlayerData player,
            bool showActive,
            float flexibleWidth,
            bool alignRight = false)
        {
            var root = AppUIFactory.CreateRect("PlayerIdentity", parent);
            var rootLayout = AppUIFactory.AddLayoutElement(root.gameObject, 52f);
            rootLayout.flexibleWidth = flexibleWidth;
            AppUIFactory.AddHorizontalLayout(root.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            if (!alignRight)
            {
                BuildAvatarWithActive(root, player, showActive);
                BuildNameColumn(root, player, TextAlignmentOptions.Left);
                return;
            }

            BuildNameColumn(root, player, TextAlignmentOptions.Right);
            BuildAvatarWithActive(root, player, showActive);
        }

        private static void BuildAvatarWithActive(Transform parent, GamePlayerData player, bool showActive)
        {
            var avatar = GameRoomUi.CreateAvatar("Avatar", parent, player, 42f);
            if (!showActive || player == null || !player.active)
            {
                return;
            }

            var dot = AppUIFactory.CreateImage("ActiveDot", avatar.transform, GameRoomStyle.RedPiece);
            dot.rectTransform.anchorMin = new Vector2(1f, 1f);
            dot.rectTransform.anchorMax = new Vector2(1f, 1f);
            dot.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            dot.rectTransform.anchoredPosition = new Vector2(-1f, -1f);
            dot.rectTransform.sizeDelta = new Vector2(8f, 8f);
        }

        private static void BuildNameColumn(Transform parent, GamePlayerData player, TextAlignmentOptions alignment)
        {
            var column = AppUIFactory.CreateRect("NameColumn", parent);
            var columnLayout = AppUIFactory.AddLayoutElement(column.gameObject, 48f);
            columnLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(column.gameObject, 2f, new RectOffset(0, 0, 2, 0), TextAnchor.MiddleLeft);

            var name = GameRoomUi.CreateLabel(
                "Name",
                column,
                player != null ? player.displayName ?? string.Empty : string.Empty,
                20f,
                GameRoomStyle.Gold,
                FontStyles.Bold,
                alignment);
            name.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(name.gameObject, 24f);

            var rankText = ResolveRankText(player);
            var rank = GameRoomUi.CreateLabel(
                "Rank",
                column,
                rankText,
                12f,
                GameRoomStyle.Alpha(GameRoomStyle.Gold, 0.72f),
                FontStyles.Normal,
                alignment);
            rank.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(rank.gameObject, 18f);
        }

        private static void BuildRoundBadge(Transform parent, string text, float width)
        {
            var badge = AppUIFactory.CreateRect("RoundBadge", parent);
            AppUIFactory.AddLayoutElement(badge.gameObject, 48f, width);
            AppUIFactory.AddVerticalLayout(badge.gameObject, 0f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);

            var label = GameRoomUi.CreateLabel(
                "Round",
                badge,
                text ?? string.Empty,
                16f,
                GameRoomStyle.Gold,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(label.gameObject, 28f);
        }

        private static void BuildTimer(Transform parent, string text, bool active)
        {
            var timer = GameRoomUi.CreateLabel(
                "Timer",
                parent,
                text ?? string.Empty,
                30f,
                active ? GameRoomStyle.Gold : GameRoomStyle.Alpha(GameRoomStyle.Gold, 0.58f),
                FontStyles.Bold,
                TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(timer.gameObject, 46f, 84f);
            timer.maxVisibleLines = 1;
        }

        private static void BuildSpectatorCenter(Transform parent, GameRoomData data)
        {
            var center = AppUIFactory.CreateRect("MatchCenter", parent);
            AppUIFactory.AddLayoutElement(center.gameObject, 54f, 86f);
            AppUIFactory.AddVerticalLayout(center.gameObject, 0f, new RectOffset(0, 0, 2, 0), TextAnchor.MiddleCenter);

            var round = GameRoomUi.CreateLabel(
                "Round",
                center,
                data != null ? data.roundText ?? string.Empty : string.Empty,
                14f,
                GameRoomStyle.Gold,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            round.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(round.gameObject, 24f);

            var timer = GameRoomUi.CreateLabel(
                "Countdown",
                center,
                data != null ? data.countdownText ?? string.Empty : string.Empty,
                18f,
                GameRoomStyle.Gold,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            timer.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(timer.gameObject, 26f);
        }

        private static string ResolveRankText(GamePlayerData player)
        {
            if (player == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(player.sideText))
            {
                return player.rank ?? string.Empty;
            }

            return string.IsNullOrEmpty(player.rank)
                ? player.sideText
                : player.rank + " · " + player.sideText;
        }
    }
}
