using NewsFramework.Data.GameRoom;
using NewsFramework.UI.Base;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.GameRoom
{
    internal static class GameRoomOverlayViews
    {
        public static void BuildCapturedPieces(RectTransform boardSlot, GameRoomData data)
        {
            if (boardSlot == null || data == null)
            {
                return;
            }

            BuildCapturedColumn("CapturedRed", boardSlot, data.redCapturedPieces, true);
            BuildCapturedColumn("CapturedBlack", boardSlot, data.blackCapturedPieces, false);
        }

        public static void BuildDanmaku(RectTransform boardRoot, GameRoomData data)
        {
            if (boardRoot == null || data == null || data.danmaku == null)
            {
                return;
            }

            var layer = AppUIFactory.CreateRect("DanmakuLayer", boardRoot);
            AppUIFactory.Stretch(layer);
            layer.SetAsLastSibling();

            for (var i = 0; i < data.danmaku.Count; i++)
            {
                var item = data.danmaku[i];
                if (item == null)
                {
                    continue;
                }

                var label = GameRoomUi.CreateLabel(
                    "Danmaku_" + i,
                    layer,
                    item.text ?? string.Empty,
                    15f,
                    GameRoomStyle.Alpha(Color.white, 0.76f),
                    FontStyles.Bold,
                    TextAlignmentOptions.Left);
                label.raycastTarget = false;
                label.rectTransform.anchorMin = new Vector2(0f, 1f);
                label.rectTransform.anchorMax = new Vector2(0f, 1f);
                label.rectTransform.pivot = new Vector2(0f, 1f);
                label.rectTransform.anchoredPosition = new Vector2(64f + item.offset, -24f - item.track * 40f);
                label.rectTransform.sizeDelta = new Vector2(220f, 24f);
            }
        }

        public static void BuildStatusBanner(Transform parent, string text)
        {
            var root = AppUIFactory.CreateRect("StatusBanner", parent);
            AppUIFactory.AddLayoutElement(root.gameObject, 32f);

            var badge = GameRoomUi.CreatePanel("Badge", root, GameRoomStyle.Alpha(GameRoomStyle.Gold, 0.22f));
            badge.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            badge.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            badge.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            badge.rectTransform.anchoredPosition = Vector2.zero;
            badge.rectTransform.sizeDelta = new Vector2(170f, 26f);

            var label = GameRoomUi.CreateLabel(
                "Label",
                badge.transform,
                text ?? string.Empty,
                16f,
                GameRoomStyle.BlackPiece,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);
        }

        private static void BuildCapturedColumn(
            string name,
            RectTransform parent,
            System.Collections.Generic.List<GameCapturedPieceData> pieces,
            bool left)
        {
            var root = AppUIFactory.CreateRect(name, parent);
            root.anchorMin = new Vector2(left ? 0f : 1f, 0.5f);
            root.anchorMax = new Vector2(left ? 0f : 1f, 0.5f);
            root.pivot = new Vector2(left ? 0f : 1f, 0.5f);
            root.anchoredPosition = new Vector2(left ? 2f : -2f, 0f);
            root.sizeDelta = new Vector2(18f, 96f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 4f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleCenter);

            if (pieces == null)
            {
                return;
            }

            for (var i = 0; i < pieces.Count; i++)
            {
                var piece = pieces[i];
                var color = piece != null && piece.side == GamePieceSide.Red
                    ? GameRoomStyle.Alpha(GameRoomStyle.RedPiece, 0.5f)
                    : GameRoomStyle.Alpha(Color.black, 0.5f);
                var chip = GameRoomUi.CreatePanel("Captured_" + i, root, color);
                AppUIFactory.AddLayoutElement(chip.gameObject, 18f, 18f);
                var label = GameRoomUi.CreateLabel(
                    "Label",
                    chip.transform,
                    piece != null ? piece.text ?? string.Empty : string.Empty,
                    10f,
                    piece != null && piece.side == GamePieceSide.Red ? Color.white : GameRoomStyle.GoldMuted,
                    FontStyles.Bold,
                    TextAlignmentOptions.Center);
                AppUIFactory.Stretch(label.rectTransform);
            }
        }
    }
}
