using NewsFramework.Data.GameRoom;
using NewsFramework.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.GameRoom
{
    /// <summary>棋盘视图。棋盘使用纹理贴图，棋子用代码构造。</summary>
    public sealed class ChessBoardView : MonoBehaviour
    {
        private RectTransform boardRoot;
        private static Sprite _boardSprite;

        public RectTransform BoardRoot => boardRoot;

        public void Build(RectTransform parent, GameRoomData data, bool darkBoard)
        {
            boardRoot = AppUIFactory.CreateRect("ChessBoard", parent);
            boardRoot.anchorMin = new Vector2(0.5f, 0.5f);
            boardRoot.anchorMax = new Vector2(0.5f, 0.5f);
            boardRoot.pivot = new Vector2(0.5f, 0.5f);
            boardRoot.anchoredPosition = Vector2.zero;
            boardRoot.sizeDelta = new Vector2(GameRoomStyle.BoardWidth, GameRoomStyle.BoardHeight);

            var boardImage = boardRoot.gameObject.AddComponent<Image>();
            boardImage.sprite = LoadBoardSprite();
            boardImage.color = darkBoard
                ? GameRoomStyle.Alpha(GameRoomStyle.BoardDeep, 0.85f)
                : Color.white;
            boardImage.type = Image.Type.Simple;

            BuildMoveMarkers(data);
            BuildPieces(data, darkBoard);
        }

        private static Sprite LoadBoardSprite()
        {
            if (_boardSprite != null) return _boardSprite;
            var tex = Resources.Load<Texture2D>("Textures/Chess/chessboard");
            if (tex == null)
            {
                Debug.LogError("[ChessBoardView] Board texture not found at Resources/Textures/Chess/chessboard. " +
                    "Board will appear as a solid color. Reimport the texture asset.");
                return null;
            }
            _boardSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            return _boardSprite;
        }

        private void BuildMoveMarkers(GameRoomData data)
        {
            if (data?.moveMarkers == null) return;
            foreach (var marker in data.moveMarkers)
            {
                var pos = GameRoomStyle.GridToPixel(marker.x, marker.y, GameRoomStyle.MoveMarkerSize);
                var dot = AppUIFactory.CreateImage(
                    "MoveMarker_" + marker.x + "_" + marker.y, boardRoot,
                    GameRoomStyle.Alpha(GameRoomStyle.Gold, marker.alpha));
                dot.raycastTarget = false;
                ChessBoardRenderer.SetAbsolute(dot.rectTransform, pos.x, pos.y,
                    GameRoomStyle.MoveMarkerSize, GameRoomStyle.MoveMarkerSize);
            }
        }

        private void BuildPieces(GameRoomData data, bool darkBoard)
        {
            if (data?.pieces == null) return;
            var pieceSize = darkBoard ? GameRoomStyle.PieceSizeDark : GameRoomStyle.PieceSizeLight;
            foreach (var piece in data.pieces)
                CreatePiece(piece, pieceSize, darkBoard);
        }

        private void CreatePiece(GamePieceData piece, float pieceSize, bool darkBoard)
        {
            if (piece == null) return;
            var pos = GameRoomStyle.GridToPixel(piece.x, piece.y, pieceSize);
            var btn = AppUIFactory.CreateButton(
                "Piece_" + piece.pieceId, boardRoot,
                ResolvePieceBackground(piece, darkBoard),
                () => Debug.Log("GameRoom piece tapped: " + piece.pieceId));
            var rect = btn.GetComponent<RectTransform>();
            ChessBoardRenderer.SetAbsolute(rect, pos.x, pos.y, pieceSize, pieceSize);

            var border = AppUIFactory.CreateImage("Border", btn.transform,
                piece.side == GamePieceSide.Red
                    ? GameRoomStyle.Alpha(GameRoomStyle.RedPiece, 0.18f)
                    : GameRoomStyle.Alpha(GameRoomStyle.BlackPiece, darkBoard ? 0.55f : 0.22f));
            AppUIFactory.Stretch(border.rectTransform);
            border.raycastTarget = false;

            var fontSize = darkBoard ? GameRoomStyle.PieceFontSizeDark : GameRoomStyle.PieceFontSizeLight;
            var label = AppUIFactory.CreateText("Label", btn.transform,
                piece.text ?? string.Empty, fontSize,
                ResolvePieceTextColor(piece, darkBoard),
                FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);

            if (!piece.highlighted) return;

            var hl = AppUIFactory.CreateImage("Highlight", btn.transform,
                GameRoomStyle.Alpha(GameRoomStyle.Gold, 0.28f));
            hl.rectTransform.anchorMin = Vector2.zero;
            hl.rectTransform.anchorMax = Vector2.one;
            hl.rectTransform.offsetMin = new Vector2(-4f, -4f);
            hl.rectTransform.offsetMax = new Vector2(4f, 4f);
            hl.raycastTarget = false;
            hl.transform.SetAsFirstSibling();
        }

        private static Color ResolvePieceBackground(GamePieceData piece, bool darkBoard)
        {
            if (piece == null) return GameRoomStyle.PieceCream;
            if (!darkBoard) return GameRoomStyle.PieceCream;
            return piece.side == GamePieceSide.Red
                ? AppUIFactory.Tint(GameRoomStyle.PieceCream, 0.98f)
                : GameRoomStyle.BlackPiece;
        }

        private static Color ResolvePieceTextColor(GamePieceData piece, bool darkBoard)
        {
            if (piece == null) return GameRoomStyle.BlackPiece;
            if (piece.side == GamePieceSide.Red) return GameRoomStyle.RedPiece;
            return darkBoard ? Color.white : GameRoomStyle.BlackPiece;
        }
    }
}