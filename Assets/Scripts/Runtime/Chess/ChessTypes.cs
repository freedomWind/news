using System;
using System.Collections.Generic;

namespace NewsFramework.Simulation.Chess
{
    /// <summary>棋子颜色（红方先手）</summary>
    public enum ChessSide
    {
        Red,   // 红方（下方，先手）
        Black  // 黑方（上方，后手）
    }

    /// <summary>棋子类型</summary>
    public enum ChessPieceType
    {
        King,     // 将/帅
        Advisor,  // 士/仕
        Bishop,   // 象/相
        Knight,   // 马
        Rook,     // 车
        Cannon,   // 炮
        Pawn      // 兵/卒
    }

    /// <summary>棋盘上的一个棋子</summary>
    public readonly struct ChessPiece : IEquatable<ChessPiece>
    {
        public static readonly ChessPiece None = default;

        public readonly ChessSide Side;
        public readonly ChessPieceType Type;
        public readonly bool IsValid;

        public ChessPiece(ChessSide side, ChessPieceType type)
        {
            Side = side;
            Type = type;
            IsValid = true;
        }

        public bool IsRed => IsValid && Side == ChessSide.Red;
        public bool IsBlack => IsValid && Side == ChessSide.Black;

        public char ToChar() => (!IsValid) ? '．' : (Side, Type) switch
        {
            (ChessSide.Red, ChessPieceType.King)   => '帅',
            (ChessSide.Red, ChessPieceType.Advisor)=> '仕',
            (ChessSide.Red, ChessPieceType.Bishop) => '相',
            (ChessSide.Red, ChessPieceType.Knight) => '马',
            (ChessSide.Red, ChessPieceType.Rook)   => '车',
            (ChessSide.Red, ChessPieceType.Cannon) => '炮',
            (ChessSide.Red, ChessPieceType.Pawn)   => '兵',
            (ChessSide.Black, ChessPieceType.King)   => '将',
            (ChessSide.Black, ChessPieceType.Advisor)=> '士',
            (ChessSide.Black, ChessPieceType.Bishop) => '象',
            (ChessSide.Black, ChessPieceType.Knight) => '马',
            (ChessSide.Black, ChessPieceType.Rook)   => '车',
            (ChessSide.Black, ChessPieceType.Cannon) => '炮',
            (ChessSide.Black, ChessPieceType.Pawn)   => '卒',
            _ => '?'
        };

        public bool Equals(ChessPiece other) => Side == other.Side && Type == other.Type && IsValid == other.IsValid;
        public override bool Equals(object obj) => obj is ChessPiece other && Equals(other);
        public override int GetHashCode() => HashCode.Combine((int)Side, (int)Type, IsValid);
        public static bool operator ==(ChessPiece a, ChessPiece b) => a.Equals(b);
        public static bool operator !=(ChessPiece a, ChessPiece b) => !a.Equals(b);
        public override string ToString() => IsValid ? $"{Side} {ToChar()}" : "None";
    }

    /// <summary>棋盘坐标 (row=0..9, col=0..8)，row=0 黑方底线，row=9 红方底线</summary>
    public readonly struct ChessPos : IEquatable<ChessPos>
    {
        public readonly int Row;
        public readonly int Col;
        public ChessPos(int row, int col) { Row = row; Col = col; }

        public bool IsValid => Row >= 0 && Row <= 9 && Col >= 0 && Col <= 8;
        public bool IsRedSide => Row >= 5 && Row <= 9;
        public bool IsBlackSide => Row >= 0 && Row <= 4;
        public bool IsInPalace => Col >= 3 && Col <= 5 && ((Row >= 0 && Row <= 2) || (Row >= 7 && Row <= 9));
        public int ManhattanDistance(ChessPos other) => Math.Abs(Row - other.Row) + Math.Abs(Col - other.Col);

        public bool Equals(ChessPos other) => Row == other.Row && Col == other.Col;
        public override bool Equals(object obj) => obj is ChessPos other && Equals(other);
        public override int GetHashCode() => Row * 13 + Col;
        public static bool operator ==(ChessPos a, ChessPos b) => a.Equals(b);
        public static bool operator !=(ChessPos a, ChessPos b) => !a.Equals(b);
        public override string ToString() => $"({Row},{Col})";
    }

    /// <summary>一步走法</summary>
    public readonly struct ChessMove : IEquatable<ChessMove>
    {
        public readonly ChessPos From;
        public readonly ChessPos To;
        public readonly ChessPiece Piece;
        public readonly ChessPiece Captured;

        public ChessMove(ChessPos from, ChessPos to, ChessPiece piece, ChessPiece captured)
        {
            From = from; To = to; Piece = piece; Captured = captured;
        }
        public bool IsCapture => Captured.IsValid;

        public bool Equals(ChessMove other) => From == other.From && To == other.To && Piece.Equals(other.Piece) && Captured.Equals(other.Captured);
        public override bool Equals(object obj) => obj is ChessMove other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(From, To, Piece, Captured);
        public static bool operator ==(ChessMove a, ChessMove b) => a.Equals(b);
        public static bool operator !=(ChessMove a, ChessMove b) => !a.Equals(b);
        public override string ToString()
        {
            var cap = IsCapture ? $"x{Captured.ToChar()}" : "";
            return $"{Piece.ToChar()}{From}->{To}{cap}";
        }
    }
}
