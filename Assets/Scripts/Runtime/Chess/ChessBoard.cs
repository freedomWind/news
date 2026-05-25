using System;
using System.Collections.Generic;
using System.Text;

namespace NewsFramework.Simulation.Chess
{
    /// <summary>棋盘：10行×9列。row=0 黑方底线，row=9 红方底线。</summary>
    public sealed class ChessBoard
    {
        public const int RowCount = 10;
        public const int ColCount = 9;

        private readonly ChessPiece[,] _board = new ChessPiece[RowCount, ColCount];
        private ChessPos _redKingPos = new ChessPos(9, 4);
        private ChessPos _blackKingPos = new ChessPos(0, 4);
        private bool _redKingExists = true;
        private bool _blackKingExists = true;

        public ChessSide SideToMove { get; private set; } = ChessSide.Red;

        public static ChessBoard CreateStandard() => FromFen("rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w");
        public static ChessBoard FromFen(string fen) { var b = new ChessBoard(); b.ParseFen(fen); return b; }

        public ChessPiece this[int row, int col] =>
            (row >= 0 && row < RowCount && col >= 0 && col < ColCount) ? _board[row, col] : ChessPiece.None;
        public ChessPiece GetPiece(ChessPos pos) => this[pos.Row, pos.Col];
        public ChessPos GetKingPos(ChessSide side) => side == ChessSide.Red ? _redKingPos : _blackKingPos;
        public bool KingExists(ChessSide side) => side == ChessSide.Red ? _redKingExists : _blackKingExists;

        public ChessBoard Clone()
        {
            var clone = new ChessBoard();
            Array.Copy(_board, clone._board, _board.Length);
            clone._redKingPos = _redKingPos;
            clone._blackKingPos = _blackKingPos;
            clone._redKingExists = _redKingExists;
            clone._blackKingExists = _blackKingExists;
            clone.SideToMove = SideToMove;
            return clone;
        }

        /// <summary>执行走法（不校验合法性），返回走法对象</summary>
        public ChessMove MakeMove(ChessPos from, ChessPos to)
        {
            var piece = _board[from.Row, from.Col];
            var captured = _board[to.Row, to.Col];
            if (captured.IsValid && captured.Type == ChessPieceType.King)
            {
                if (captured.Side == ChessSide.Red) _redKingExists = false;
                else _blackKingExists = false;
            }
            _board[to.Row, to.Col] = piece;
            _board[from.Row, from.Col] = ChessPiece.None;
            if (piece.Type == ChessPieceType.King)
            {
                if (piece.Side == ChessSide.Red) _redKingPos = to;
                else _blackKingPos = to;
            }
            SideToMove = SideToMove == ChessSide.Red ? ChessSide.Black : ChessSide.Red;
            return new ChessMove(from, to, piece, captured);
        }

        /// <summary>撤销走法</summary>
        public void UndoMove(ChessMove move)
        {
            _board[move.From.Row, move.From.Col] = move.Piece;
            _board[move.To.Row, move.To.Col] = move.Captured;
            if (move.Piece.Type == ChessPieceType.King)
            {
                if (move.Piece.Side == ChessSide.Red) _redKingPos = move.From;
                else _blackKingPos = move.From;
            }
            if (move.Captured.IsValid && move.Captured.Type == ChessPieceType.King)
            {
                if (move.Captured.Side == ChessSide.Red) _redKingExists = true;
                else _blackKingExists = true;
            }
            SideToMove = SideToMove == ChessSide.Red ? ChessSide.Black : ChessSide.Red;
        }

        public string ToFen()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < RowCount; row++)
            {
                int empty = 0;
                for (int col = 0; col < ColCount; col++)
                {
                    var piece = _board[row, col];
                    if (!piece.IsValid) { empty++; }
                    else
                    {
                        if (empty > 0) { sb.Append(empty); empty = 0; }
                        sb.Append(PieceToFenChar(piece));
                    }
                }
                if (empty > 0) sb.Append(empty);
                if (row < RowCount - 1) sb.Append('/');
            }
            sb.Append(SideToMove == ChessSide.Red ? " w" : " b");
            return sb.ToString();
        }

        public string ToAsciiBoard()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColCount; col++)
                    sb.Append(_board[row, col].ToChar());
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void ParseFen(string fen)
        {
            for (int r = 0; r < RowCount; r++)
                for (int c = 0; c < ColCount; c++)
                    _board[r, c] = ChessPiece.None;

            var parts = fen.Trim().Split(' ');
            var boardPart = parts[0];
            int row = 0, col = 0;
            foreach (char ch in boardPart)
            {
                if (ch == '/') { row++; col = 0; continue; }
                if (char.IsDigit(ch)) { col += ch - '0'; continue; }
                _board[row, col] = FenCharToPiece(ch);
                if (_board[row, col].Type == ChessPieceType.King)
                {
                    if (_board[row, col].Side == ChessSide.Red)
                    { _redKingPos = new ChessPos(row, col); _redKingExists = true; }
                    else
                    { _blackKingPos = new ChessPos(row, col); _blackKingExists = true; }
                }
                col++;
            }
            SideToMove = parts.Length > 1 && parts[1] == "b" ? ChessSide.Black : ChessSide.Red;
        }

        private static char PieceToFenChar(ChessPiece p) => (p.Side, p.Type) switch
        {
            (ChessSide.Red, ChessPieceType.King)   => 'K',
            (ChessSide.Red, ChessPieceType.Advisor)=> 'A',
            (ChessSide.Red, ChessPieceType.Bishop) => 'B',
            (ChessSide.Red, ChessPieceType.Knight) => 'N',
            (ChessSide.Red, ChessPieceType.Rook)   => 'R',
            (ChessSide.Red, ChessPieceType.Cannon) => 'C',
            (ChessSide.Red, ChessPieceType.Pawn)   => 'P',
            (ChessSide.Black, ChessPieceType.King)   => 'k',
            (ChessSide.Black, ChessPieceType.Advisor)=> 'a',
            (ChessSide.Black, ChessPieceType.Bishop) => 'b',
            (ChessSide.Black, ChessPieceType.Knight) => 'n',
            (ChessSide.Black, ChessPieceType.Rook)   => 'r',
            (ChessSide.Black, ChessPieceType.Cannon) => 'c',
            (ChessSide.Black, ChessPieceType.Pawn)   => 'p',
            _ => '?'
        };

        private static ChessPiece FenCharToPiece(char ch) => ch switch
        {
            'K' => new ChessPiece(ChessSide.Red, ChessPieceType.King),
            'A' => new ChessPiece(ChessSide.Red, ChessPieceType.Advisor),
            'B' => new ChessPiece(ChessSide.Red, ChessPieceType.Bishop),
            'N' => new ChessPiece(ChessSide.Red, ChessPieceType.Knight),
            'R' => new ChessPiece(ChessSide.Red, ChessPieceType.Rook),
            'C' => new ChessPiece(ChessSide.Red, ChessPieceType.Cannon),
            'P' => new ChessPiece(ChessSide.Red, ChessPieceType.Pawn),
            'k' => new ChessPiece(ChessSide.Black, ChessPieceType.King),
            'a' => new ChessPiece(ChessSide.Black, ChessPieceType.Advisor),
            'b' => new ChessPiece(ChessSide.Black, ChessPieceType.Bishop),
            'n' => new ChessPiece(ChessSide.Black, ChessPieceType.Knight),
            'r' => new ChessPiece(ChessSide.Black, ChessPieceType.Rook),
            'c' => new ChessPiece(ChessSide.Black, ChessPieceType.Cannon),
            'p' => new ChessPiece(ChessSide.Black, ChessPieceType.Pawn),
            _ => ChessPiece.None
        };
    }
}
