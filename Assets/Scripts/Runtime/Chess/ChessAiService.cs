using System;
using System.Collections.Generic;

namespace NewsFramework.Simulation.Chess
{
    /// <summary>中国象棋 AI，使用 Negamax + Alpha-Beta 剪枝。</summary>
    public static class ChessAiService
    {
        private static readonly Dictionary<ChessPieceType, int> PieceValues = new()
        {
            { ChessPieceType.King, 100000 },
            { ChessPieceType.Rook, 900 },
            { ChessPieceType.Cannon, 450 },
            { ChessPieceType.Knight, 400 },
            { ChessPieceType.Bishop, 200 },
            { ChessPieceType.Advisor, 200 },
            { ChessPieceType.Pawn, 100 }
        };

        public static int Evaluate(ChessBoard board)
        {
            int score = 0;
            for (int r = 0; r < ChessBoard.RowCount; r++)
                for (int c = 0; c < ChessBoard.ColCount; c++)
                {
                    var p = board[r, c];
                    if (!p.IsValid) continue;
                    int v = PieceValues[p.Type];
                    if (p.Type == ChessPieceType.Pawn)
                    {
                        bool crossed = p.Side == ChessSide.Red ? r <= 4 : r >= 5;
                        if (crossed) v += 40;
                    }
                    score += p.Side == ChessSide.Red ? v : -v;
                }
            return score;
        }

        /// <summary>搜索最佳走法。使用 Negamax + Alpha-Beta 剪枝。</summary>
        public static ChessMove? SearchBestMove(ChessBoard board, ChessSide side, int depth = 3)
        {
            var moves = ChessRuleEngine.GenerateLegalMoves(board);
            if (moves.Count == 0) return null;

            // 走法排序：吃子走法优先（提高剪枝效率）
            moves.Sort((a, b) =>
            {
                int va = a.IsCapture ? PieceValues.GetValueOrDefault(a.Captured.Type, 0) : 0;
                int vb = b.IsCapture ? PieceValues.GetValueOrDefault(b.Captured.Type, 0) : 0;
                return vb.CompareTo(va);
            });

            ChessMove? bestMove = null;
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            const int beta = int.MaxValue;

            foreach (var move in moves)
            {
                board.MakeMove(move.From, move.To);
                int score = -Negamax(board, depth - 1, -beta, -alpha, side == ChessSide.Red ? ChessSide.Black : ChessSide.Red);
                board.UndoMove(move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
                if (score > alpha)
                    alpha = score;
            }
            return bestMove;
        }

        private static int Negamax(ChessBoard board, int depth, int alpha, int beta, ChessSide side)
        {
            // 叶子节点：静态评估
            if (depth <= 0)
                return side == ChessSide.Red ? Evaluate(board) : -Evaluate(board);

            var moves = ChessRuleEngine.GenerateLegalMoves(board);
            // 无合法走法：被将杀或困毙
            if (moves.Count == 0)
                return -99999 + (3 - depth); // 越早被将杀分数越低

            // 走法排序
            moves.Sort((a, b) =>
            {
                int va = a.IsCapture ? PieceValues.GetValueOrDefault(a.Captured.Type, 0) : 0;
                int vb = b.IsCapture ? PieceValues.GetValueOrDefault(b.Captured.Type, 0) : 0;
                return vb.CompareTo(va);
            });

            int bestScore = int.MinValue;
            var nextSide = side == ChessSide.Red ? ChessSide.Black : ChessSide.Red;

            foreach (var move in moves)
            {
                board.MakeMove(move.From, move.To);
                int score = -Negamax(board, depth - 1, -beta, -alpha, nextSide);
                board.UndoMove(move);

                if (score > bestScore)
                    bestScore = score;
                if (score > alpha)
                    alpha = score;
                if (alpha >= beta)
                    break; // Beta 剪枝
            }
            return bestScore;
        }
    }
}
