using System;
using System.Collections.Generic;
using System.Text;

namespace NewsFramework.Simulation.Chess
{
    /// <summary>棋谱记录器，支持 ICCS / 中文走法</summary>
    public sealed class ChessGameRecorder
    {
        private readonly List<ChessMove> _moves = new List<ChessMove>(256);
        public IReadOnlyList<ChessMove> Moves => _moves;
        public int MoveCount => _moves.Count;
        public ChessMove? LastMove => _moves.Count > 0 ? _moves[_moves.Count - 1] : null;
        public void Record(ChessMove move) => _moves.Add(move);
        public ChessMove? UndoLast()
        {
            if (_moves.Count == 0) return null;
            var last = _moves[_moves.Count - 1];
            _moves.RemoveAt(_moves.Count - 1);
            return last;
        }

        public static string MoveToIccs(ChessMove move) =>
            $"{(char)('a' + move.From.Col)}{(char)('0' + (9 - move.From.Row))}" +
            $"{(char)('a' + move.To.Col)}{(char)('0' + (9 - move.To.Row))}";

        public static ChessMove? IccsToMove(ChessBoard board, string iccs)
        {
            if (string.IsNullOrEmpty(iccs) || iccs.Length < 4) return null;
            var from = new ChessPos(9 - (iccs[1] - '0'), iccs[0] - 'a');
            var to = new ChessPos(9 - (iccs[3] - '0'), iccs[2] - 'a');
            var piece = board.GetPiece(from);
            if (!piece.IsValid) return null;
            return new ChessMove(from, to, piece, board.GetPiece(to));
        }

        public static string MoveToChinese(ChessMove move)
        {
            // 简易中文走法
            var cn = move.Piece.ToChar().ToString();
            var fromCol = ColToChinese(move.From.Col, move.Piece.Side);
            var toCol = ColToChinese(move.To.Col, move.Piece.Side);
            int diff = move.To.Row - move.From.Row;
            string action;
            if (diff == 0) action = "平";
            else if ((move.Piece.Side == ChessSide.Red && diff < 0) ||
                     (move.Piece.Side == ChessSide.Black && diff > 0)) action = "进";
            else action = "退";
            string target = move.Piece.Type is ChessPieceType.Knight or ChessPieceType.Bishop or ChessPieceType.Advisor
                ? toCol : ColToChinese(Math.Abs(diff), move.Piece.Side);
            return $"{cn}{fromCol}{action}{target}";
        }

        private static string ColToChinese(int col, ChessSide side)
        {
            if (side == ChessSide.Red)
            {
                var d = new[] { "九", "八", "七", "六", "五", "四", "三", "二", "一" };
                return col >= 0 && col <= 8 ? d[col] : col.ToString();
            }
            return col >= 0 && col <= 8 ? (9 - col).ToString() : col.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _moves.Count; i += 2)
            {
                sb.Append($"{i / 2 + 1}. {MoveToChinese(_moves[i])}");
                if (i + 1 < _moves.Count) sb.Append($"  {MoveToChinese(_moves[i + 1])}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
