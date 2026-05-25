using System;
using NewsFramework.Data.GameRoom;
using NewsFramework.UI.Base;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.GameRoom
{
    internal static class GameRoomActionBarView
    {
        public static void BuildPlayerActions(Transform parent, GameRoomData data, Action<GameRoomActionData> onAction)
        {
            var root = AppUIFactory.CreateRect("PlayerActionBar", parent);
            AppUIFactory.AddLayoutElement(root.gameObject, 88f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 10f, new RectOffset(16, 16, 18, 18), TextAnchor.MiddleCenter);

            if (data == null || data.actions == null)
            {
                return;
            }

            foreach (var action in data.actions)
            {
                var button = GameRoomUi.CreateTextButton(
                    "Action_" + action.actionId,
                    root,
                    action.label ?? string.Empty,
                    GameRoomStyle.PanelLight,
                    GameRoomStyle.BlackPiece,
                    16f,
                    () => onAction?.Invoke(action));
                AppUIFactory.AddLayoutElement(button.gameObject, 48f, 72f);
            }
        }

        public static void BuildSpectatorActions(Transform parent, GameRoomData data, Action<GameRoomActionData> onAction)
        {
            var root = GameRoomUi.CreatePanel("SpectatorInteractionBar", parent, GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0.96f));
            AppUIFactory.AddLayoutElement(root.gameObject, 110f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 10f, new RectOffset(16, 16, 12, 10), TextAnchor.MiddleCenter);

            if (data == null || data.mode != GameRoomMode.Replay)
            {
                BuildCommentRow(root.transform, onAction);
            }

            BuildSpectatorLinkRow(root.transform, data, onAction);
        }

        private static void BuildCommentRow(Transform parent, Action<GameRoomActionData> onAction)
        {
            var row = AppUIFactory.CreateRect("CommentRow", parent);
            AppUIFactory.AddLayoutElement(row.gameObject, 40f);
            AppUIFactory.AddHorizontalLayout(row.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            var input = AppUIFactory.CreateButton(
                "CommentInput",
                row,
                GameRoomStyle.Alpha(Color.white, 0.1f),
                () => Debug.Log("GameRoom spectator comment input tapped."));
            var inputLayout = AppUIFactory.AddLayoutElement(input.gameObject, 40f);
            inputLayout.flexibleWidth = 1f;
            var placeholder = GameRoomUi.CreateLabel(
                "Placeholder",
                input.transform,
                "说点什么...",
                15f,
                GameRoomStyle.Alpha(Color.white, 0.72f),
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft);
            placeholder.rectTransform.anchorMin = Vector2.zero;
            placeholder.rectTransform.anchorMax = Vector2.one;
            placeholder.rectTransform.offsetMin = new Vector2(14f, 0f);
            placeholder.rectTransform.offsetMax = new Vector2(-12f, 0f);

            var sendAction = new GameRoomActionData { actionId = "send_comment", label = "发送" };
            var send = GameRoomUi.CreateTextButton(
                "SendButton",
                row,
                "发送",
                GameRoomStyle.PanelLight,
                GameRoomStyle.WalnutDark,
                14f,
                () => onAction?.Invoke(sendAction));
            AppUIFactory.AddLayoutElement(send.gameObject, 36f, 64f);

            var flowerAction = new GameRoomActionData { actionId = "flower", label = "献花" };
            var flower = GameRoomUi.CreateTextButton(
                "FlowerButton",
                row,
                "花 128",
                GameRoomStyle.PanelLight,
                GameRoomStyle.WalnutDark,
                14f,
                () => onAction?.Invoke(flowerAction));
            AppUIFactory.AddLayoutElement(flower.gameObject, 36f, 70f);
        }

        private static void BuildSpectatorLinkRow(Transform parent, GameRoomData data, Action<GameRoomActionData> onAction)
        {
            var row = AppUIFactory.CreateRect("SpectatorActionLinks", parent);
            AppUIFactory.AddLayoutElement(row.gameObject, 32f);
            AppUIFactory.AddHorizontalLayout(row.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);

            if (data == null || data.actions == null)
            {
                return;
            }

            foreach (var action in data.actions)
            {
                var label = string.IsNullOrEmpty(action.icon)
                    ? action.label
                    : action.icon + " " + action.label;
                var button = GameRoomUi.CreateTextButton(
                    "SpectatorAction_" + action.actionId,
                    row,
                    label ?? string.Empty,
                    GameRoomStyle.Alpha(GameRoomStyle.WalnutDark, 0f),
                    GameRoomStyle.GoldMuted,
                    13f,
                    () => onAction?.Invoke(action));
                var layout = AppUIFactory.AddLayoutElement(button.gameObject, 30f);
                layout.flexibleWidth = 1f;
            }
        }
    }
}
