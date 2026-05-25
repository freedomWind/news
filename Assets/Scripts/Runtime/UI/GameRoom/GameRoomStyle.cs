using NewsFramework.UI.Theme;
using UnityEngine;

namespace NewsFramework.UI.GameRoom
{
    internal static class GameRoomStyle
    {
        // ──── 颜色 ────
        public static readonly Color Walnut = AppTheme.Rgb(93, 64, 55);
        public static readonly Color WalnutDark = AppTheme.Rgb(62, 39, 35);
        public static readonly Color WalnutDeep = AppTheme.Rgb(44, 26, 20);
        public static readonly Color BoardWarm = AppTheme.Rgb(222, 184, 135);
        public static readonly Color BoardDeep = AppTheme.Rgb(109, 76, 65);
        public static readonly Color BoardLineDark = AppTheme.Rgb(62, 39, 35);
        public static readonly Color Gold = AppTheme.Rgb(212, 165, 116);
        public static readonly Color GoldMuted = AppTheme.Rgb(196, 168, 130);
        public static readonly Color PieceCream = AppTheme.Rgb(255, 248, 231);
        public static readonly Color PanelLight = AppTheme.Rgb(245, 240, 232);
        public static readonly Color RedPiece = AppTheme.Rgb(181, 67, 58);
        public static readonly Color BlackPiece = AppTheme.Rgb(44, 36, 22);

        // ──── 棋盘尺寸 ────
        public const float BoardWidth = 345f;
        public const float BoardHeight = 384f;

        // ──── 棋盘布局常量 ────
        /// <summary>棋盘边缘留白（像素）</summary>
        public const float BoardPadding = 19f;
        /// <summary>走子标记点直径</summary>
        public const float MoveMarkerSize = 8f;
        /// <summary>深色棋盘棋子尺寸</summary>
        public const float PieceSizeDark = 32f;
        /// <summary>浅色棋盘棋子尺寸</summary>
        public const float PieceSizeLight = 38f;
        /// <summary>深色棋盘棋子文字字号</summary>
        public const float PieceFontSizeDark = 18f;
        /// <summary>浅色棋盘棋子文字字号</summary>
        public const float PieceFontSizeLight = 22f;

        /// <summary>棋盘交叉点之间的水平步长</summary>
        public static float CellWidth => (BoardWidth - BoardPadding * 2f) / 8f;
        /// <summary>棋盘交叉点之间的垂直步长</summary>
        public static float CellHeight => (BoardHeight - BoardPadding * 2f) / 9f;

        /// <summary>根据棋盘坐标 (0-8, 0-9) 换算到像素位置（左上角为原点）</summary>
        public static Vector2 GridToPixel(int gridX, int gridY, float elementSize)
        {
            return new Vector2(
                BoardPadding + Mathf.Clamp(gridX, 0, 8) * CellWidth - elementSize * 0.5f,
                BoardPadding + Mathf.Clamp(gridY, 0, 9) * CellHeight - elementSize * 0.5f);
        }

        /// <summary>从像素坐标反算到棋盘坐标</summary>
        public static Vector2Int PixelToGrid(Vector2 pixelPos)
        {
            var x = Mathf.RoundToInt((pixelPos.x - BoardPadding) / CellWidth);
            var y = Mathf.RoundToInt((pixelPos.y - BoardPadding) / CellHeight);
            return new Vector2Int(Mathf.Clamp(x, 0, 8), Mathf.Clamp(y, 0, 9));
        }

        public static Color Alpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
