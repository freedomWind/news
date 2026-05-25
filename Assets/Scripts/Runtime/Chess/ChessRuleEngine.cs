using System;
using System.Collections.Generic;

namespace NewsFramework.Simulation.Chess
{
    /// <summary>中国象棋规则引擎。纯 C#，无 Unity 依赖。</summary>
    public static class ChessRuleEngine
    {
        /// <summary>生成当前方所有合法走法</summary>
        public static List<ChessMove> GenerateLegalMoves(ChessBoard board)
        {
            var pseudoMoves = GeneratePseudoMoves(board);
            var legalMoves = new List<ChessMove>(pseudoMoves.Count);

            foreach (var move in pseudoMoves)
            {
                board.MakeMove(move.From, move.To);
                bool inCheck = IsInCheck(board, move.Piece.Side);
                board.UndoMove(move);
                if (!inCheck) legalMoves.Add(move);
            }
            return legalMoves;
        }

        /// <summary>检查某方是否被将军</summary>
        public static bool IsInCheck(ChessBoard board, ChessSide side)
        {
            if (!board.KingExists(side)) return true;
            var kingPos = board.GetKingPos(side);
            var opponent = side == ChessSide.Red ? ChessSide.Black : ChessSide.Red;

            for (int r = 0; r < ChessBoard.RowCount; r++)
            {
                for (int c = 0; c < ChessBoard.ColCount; c++)
                {
                    var piece = board[r, c];
                    if (!piece.IsValid || piece.Side != opponent) continue;
                    var from = new ChessPos(r, c);
                    var targets = GetRawMoves(board, from, piece);
                    if (targets.Contains(kingPos)) return true;
                }
            }
            if (KingsAreFacing(board)) return true;
            return false;
        }

        /// <summary>是否将死/困毙</summary>
        public static bool IsCheckmate(ChessBoard board, ChessSide side) => GenerateLegalMoves(board).Count == 0;

        /// <summary>快速判断走法合法性</summary>
        public static bool IsMoveLegal(ChessBoard board, ChessPos from, ChessPos to)
        {
            var piece = board.GetPiece(from);
            if (!piece.IsValid || piece.Side != board.SideToMove) return false;
            var targets = GetRawMoves(board, from, piece);
            if (!targets.Contains(to)) return false;

            var move = new ChessMove(from, to, piece, board.GetPiece(to));
            board.MakeMove(move.From, move.To);
            bool inCheck = IsInCheck(board, piece.Side);
            board.UndoMove(move);
            return !inCheck;
        }

        /// <summary>子力不足和棋</summary>
        public static bool IsDrawByInsufficientMaterial(ChessBoard board)
        {
            int red = 0, black = 0;
            for (int r = 0; r < ChessBoard.RowCount; r++)
                for (int c = 0; c < ChessBoard.ColCount; c++)
                {
                    var p = board[r, c];
                    if (!p.IsValid) continue;
                    if (p.Side == ChessSide.Red) red++; else black++;
                }
            return red <= 1 && black <= 1;
        }

        /// <summary>将帅是否对面</summary>
        public static bool KingsAreFacing(ChessBoard board)
        {
            if (!board.KingExists(ChessSide.Red) || !board.KingExists(ChessSide.Black)) return false;
            var rk = board.GetKingPos(ChessSide.Red);
            var bk = board.GetKingPos(ChessSide.Black);
            if (rk.Col != bk.Col) return false;
            int min = Math.Min(rk.Row, bk.Row), max = Math.Max(rk.Row, bk.Row);
            for (int r = min + 1; r < max; r++)
                if (board[r, rk.Col].IsValid) return false;
            return true;
        }

        // ──── 各棋子伪走法 ────

        private static List<ChessMove> GeneratePseudoMoves(ChessBoard board)
        {
            var moves = new List<ChessMove>(48);
            var side = board.SideToMove;
            for (int r = 0; r < ChessBoard.RowCount; r++)
                for (int c = 0; c < ChessBoard.ColCount; c++)
                {
                    var piece = board[r, c];
                    if (!piece.IsValid || piece.Side != side) continue;
                    var from = new ChessPos(r, c);
                    foreach (var to in GetRawMoves(board, from, piece))
                        moves.Add(new ChessMove(from, to, piece, board.GetPiece(to)));
                }
            return moves;
        }

        private static List<ChessPos> GetRawMoves(ChessBoard board, ChessPos from, ChessPiece piece) => piece.Type switch
        {
            ChessPieceType.King    => GetKingMoves(board, from, piece.Side),
            ChessPieceType.Advisor => GetAdvisorMoves(board, from, piece.Side),
            ChessPieceType.Bishop  => GetBishopMoves(board, from, piece.Side),
            ChessPieceType.Knight  => GetKnightMoves(board, from, piece.Side),
            ChessPieceType.Rook    => GetRookMoves(board, from, piece.Side),
            ChessPieceType.Cannon  => GetCannonMoves(board, from, piece.Side),
            ChessPieceType.Pawn    => GetPawnMoves(board, from, piece.Side),
            _ => new List<ChessPos>(0)
        };

        private static bool InBoard(int r, int c) => r >= 0 && r <= 9 && c >= 0 && c <= 8;
        private static bool CanMoveTo(ChessBoard board, ChessPos pos, ChessSide side)
        {
            var t = board.GetPiece(pos);
            return !t.IsValid || t.Side != side;
        }

        private static List<ChessPos> GetKingMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(4);
            foreach (var (dr, dc) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                var pos = new ChessPos(from.Row + dr, from.Col + dc);
                if (pos.IsInPalace && CanMoveTo(board, pos, side)) moves.Add(pos);
            }
            return moves;
        }

        private static List<ChessPos> GetAdvisorMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(4);
            foreach (var (dr, dc) in new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) })
            {
                var pos = new ChessPos(from.Row + dr, from.Col + dc);
                if (pos.IsInPalace && CanMoveTo(board, pos, side)) moves.Add(pos);
            }
            return moves;
        }

        private static List<ChessPos> GetBishopMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(4);
            foreach (var (dr, dc) in new[] { (2, 2), (2, -2), (-2, 2), (-2, -2) })
            {
                int nr = from.Row + dr, nc = from.Col + dc;
                int er = from.Row + dr / 2, ec = from.Col + dc / 2;
                var pos = new ChessPos(nr, nc);
                if (InBoard(nr, nc) && !board[er, ec].IsValid
                    && pos.IsRedSide == (side == ChessSide.Red)
                    && CanMoveTo(board, pos, side))
                    moves.Add(pos);
            }
            return moves;
        }

        private static List<ChessPos> GetKnightMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(8);
            foreach (var (dr, dc, lr, lc) in new[] {
                (2, 1, 1, 0), (2, -1, 1, 0), (-2, 1, -1, 0), (-2, -1, -1, 0),
                (1, 2, 0, 1), (1, -2, 0, -1), (-1, 2, 0, 1), (-1, -2, 0, -1) })
            {
                int nr = from.Row + dr, nc = from.Col + dc;
                var pos = new ChessPos(nr, nc);
                if (InBoard(nr, nc) && !board[from.Row + lr, from.Col + lc].IsValid && CanMoveTo(board, pos, side))
                    moves.Add(pos);
            }
            return moves;
        }

        private static List<ChessPos> GetRookMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(17);
            foreach (var (dr, dc) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                int nr = from.Row + dr, nc = from.Col + dc;
                while (InBoard(nr, nc))
                {
                    var pos = new ChessPos(nr, nc);
                    var t = board.GetPiece(pos);
                    if (!t.IsValid) { moves.Add(pos); }
                    else { if (t.Side != side) moves.Add(pos); break; }
                    nr += dr; nc += dc;
                }
            }
            return moves;
        }

        private static List<ChessPos> GetCannonMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(17);
            foreach (var (dr, dc) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                int nr = from.Row + dr, nc = from.Col + dc;
                bool screen = false;
                while (InBoard(nr, nc))
                {
                    var pos = new ChessPos(nr, nc);
                    var t = board.GetPiece(pos);
                    if (!screen)
                    {
                        if (!t.IsValid) moves.Add(pos);
                        else screen = true;
                    }
                    else
                    {
                        if (t.IsValid) { if (t.Side != side) moves.Add(pos); break; }
                    }
                    nr += dr; nc += dc;
                }
            }
            return moves;
        }

        private static List<ChessPos> GetPawnMoves(ChessBoard board, ChessPos from, ChessSide side)
        {
            var moves = new List<ChessPos>(3);
            int fwd = side == ChessSide.Red ? -1 : 1;
            var fpos = new ChessPos(from.Row + fwd, from.Col);
            if (InBoard(fpos.Row, fpos.Col) && CanMoveTo(board, fpos, side)) moves.Add(fpos);

            bool crossed = side == ChessSide.Red ? from.Row <= 4 : from.Row >= 5;
            if (crossed)
            {
                foreach (int dc in new[] { -1, 1 })
                {
                    var spos = new ChessPos(from.Row, from.Col + dc);
                    if (InBoard(spos.Row, spos.Col) && CanMoveTo(board, spos, side)) moves.Add(spos);
                }
            }
            return moves;
        }
    }
}
