using NewsFramework.Data.GameRoom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Base
{
    public static class ChessBoardRenderer
    {
        public const float DefaultBoardWidth = 345f;
        public const float DefaultBoardHeight = 384f;
        public const float DefaultPadding = 19f;
        public const float DefaultPieceSize = 38f;

        private static Texture2D _boardTexture;

        // ──── 坐标计算 ────

        public static Vector2 ResolveBoardPoint(int x, int y, float pieceSize,
            float boardWidth = DefaultBoardWidth, float boardHeight = DefaultBoardHeight, float padding = DefaultPadding)
        {
            var xStep = (boardWidth - padding * 2f) / 8f;
            var yStep = (boardHeight - padding * 2f) / 9f;
            return new Vector2(
                padding + Mathf.Clamp(x, 0, 8) * xStep - pieceSize * 0.5f,
                padding + Mathf.Clamp(y, 0, 9) * yStep - pieceSize * 0.5f);
        }

        public static void SetAbsolute(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        // ──── 棋盘底图 ────

        public static Sprite LoadBoardSprite()
        {
            if (_boardTexture != null)
            {
                return Sprite.Create(_boardTexture,
                    new Rect(0, 0, _boardTexture.width, _boardTexture.height),
                    new Vector2(0.5f, 0.5f));
            }

            var tex = Resources.Load<Texture2D>("Textures/Chess/chessboard");
            if (tex != null)
            {
                _boardTexture = tex;
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }

            Debug.LogWarning("Chess board texture not found at Resources/Textures/Chess/chessboard");
            return null;
        }

        public static Image CreateBoardBackground(Transform parent, float width, float height)
        {
            var image = AppUIFactory.CreateImage("BoardBg", parent, Color.white);
            image.sprite = LoadBoardSprite();
            image.type = Image.Type.Simple;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(width, height);
            return image;
        }

        // ──── 棋子（非交互版） ────

        public static RectTransform CreatePieceIcon(Transform parent, GamePieceData piece, float pieceSize, bool darkBoard)
        {
            var pos = ResolveBoardPoint(piece.x, piece.y, pieceSize);

            var bg = AppUIFactory.CreateImage(
                "Piece_" + piece.pieceId,
                parent,
                ResolvePieceBackground(piece.side, darkBoard));
            bg.raycastTarget = false;
            SetAbsolute(bg.rectTransform, pos.x, pos.y, pieceSize, pieceSize);

            var label = AppUIFactory.CreateText(
                "Label",
                bg.transform,
                piece.text ?? string.Empty,
                darkBoard ? 18f : 22f,
                ResolvePieceTextColor(piece.side, darkBoard),
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);

            return bg.rectTransform;
        }

        // ──── 走棋标记 ────

        public static void CreateMoveMarker(Transform parent, int x, int y,
            float pieceSize, Color color, float alpha,
            float boardWidth = DefaultBoardWidth, float boardHeight = DefaultBoardHeight, float padding = DefaultPadding)
        {
            const float dotSize = 8f;
            var pos = ResolveBoardPoint(x, y, pieceSize, boardWidth, boardHeight, padding);
            // Center the dot relative to the piece cell center
            var dot = AppUIFactory.CreateImage(
                "MoveMarker_" + x + "_" + y,
                parent,
                new Color(color.r, color.g, color.b, alpha));
            dot.raycastTarget = false;
            SetAbsolute(dot.rectTransform,
                pos.x + pieceSize * 0.5f - dotSize * 0.5f,
                pos.y + pieceSize * 0.5f - dotSize * 0.5f,
                dotSize, dotSize);
        }

        // ──── 棋子配色 ────

        private static readonly Color PieceCream = new Color(1f, 0.973f, 0.906f, 1f);   // #FFF8E7
        private static readonly Color PieceBlack = new Color(0.173f, 0.141f, 0.086f, 1f); // #2C2416
        private static readonly Color PieceRed   = new Color(0.71f, 0.263f, 0.227f, 1f);  // #B5433A

        public static Color ResolvePieceBackground(GamePieceSide side, bool darkBoard)
        {
            if (!darkBoard) return PieceCream;
            return side == GamePieceSide.Red
                ? AppUIFactory.Tint(PieceCream, 0.98f)
                : PieceBlack;
        }

        public static Color ResolvePieceTextColor(GamePieceSide side, bool darkBoard)
        {
            if (side == GamePieceSide.Red) return PieceRed;
            return darkBoard ? Color.white : PieceBlack;
        }
    }
}
