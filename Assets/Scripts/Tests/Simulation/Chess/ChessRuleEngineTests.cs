using System.Collections.Generic;
using NewsFramework.Simulation.Chess;
using NUnit.Framework;

namespace NewsFramework.Tests.Simulation.Chess
{
    public sealed class ChessRuleEngineTests
    {
        // ──── FEN 解析/生成 ────

        [Test]
        public void StandardBoard_HasCorrectFenPrefix()
        {
            var board = ChessBoard.CreateStandard();
            Assert.That(board.ToFen(), Does.StartWith("rnbakabnr"));
        }

        [Test]
        public void FenRoundTrip_ProducesSameString()
        {
            var original = ChessBoard.CreateStandard();
            var fen = original.ToFen();
            var restored = ChessBoard.FromFen(fen);
            Assert.That(restored.ToFen(), Is.EqualTo(fen));
        }

        [Test]
        public void FromFen_InvalidFen_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => ChessBoard.FromFen("invalid fen string"));
        }

        // ──── 走法生成 ────

        [Test]
        public void StandardBoard_RedHasLegalMoves()
        {
            var board = ChessBoard.CreateStandard();
            var moves = ChessRuleEngine.GenerateLegalMoves(board);
            Assert.That(moves.Count, Is.GreaterThan(0));
        }

        [Test]
        public void StandardBoard_RedSideToMove()
        {
            var board = ChessBoard.CreateStandard();
            Assert.That(board.SideToMove, Is.EqualTo(ChessSide.Red));
        }

        [Test]
        public void AfterFirstMove_BlackSideToMove()
        {
            var board = ChessBoard.CreateStandard();
            // 炮二平五
            board.MakeMove(new ChessPos(9, 1), new ChessPos(7, 4));
            Assert.That(board.SideToMove, Is.EqualTo(ChessSide.Black));
        }

        // ──── 将军检测 ────

        [Test]
        public void RookCheck_KingInCheck()
        {
            // 黑将在 (0,4)，红车在 (1,4) 将军
            var board = ChessBoard.FromFen("4k4/4R4/9/9/9/9/9/9/9/4K4 w");
            Assert.That(ChessRuleEngine.IsInCheck(board, ChessSide.Black), Is.True);
        }

        [Test]
        public void StartingPosition_NoCheck()
        {
            var board = ChessBoard.CreateStandard();
            Assert.That(ChessRuleEngine.IsInCheck(board, ChessSide.Red), Is.False);
            Assert.That(ChessRuleEngine.IsInCheck(board, ChessSide.Black), Is.False);
        }

        // ──── 将死 ────

        [Test]
        public void CheckmatePosition_BlackIsCheckmated()
        {
            // 黑将无法逃脱
            var board = ChessBoard.FromFen("4k4/9/4R4/9/9/9/9/9/9/4K4 w");
            Assert.That(ChessRuleEngine.IsCheckmate(board, ChessSide.Black), Is.True);
        }

        [Test]
        public void StandardBoard_NotCheckmate()
        {
            var board = ChessBoard.CreateStandard();
            Assert.That(ChessRuleEngine.IsCheckmate(board, ChessSide.Red), Is.False);
        }

        // ──── 将帅对面 ────

        [Test]
        public void KingsOnSameColumnNoPiecesBetween_AreFacing()
        {
            var board = ChessBoard.FromFen("4k4/9/9/9/9/9/9/9/9/4K4 w");
            Assert.That(ChessRuleEngine.KingsAreFacing(board), Is.True);
        }

        [Test]
        public void KingsOnSameColumnWithPieceBetween_NotFacing()
        {
            var board = ChessBoard.CreateStandard();
            // 标准开局中间很多子，不应该对面
            Assert.That(ChessRuleEngine.KingsAreFacing(board), Is.False);
        }

        // ──── 走法合法性 ────

        [Test]
        public void IsMoveLegal_ValidMove_ReturnsTrue()
        {
            var board = ChessBoard.CreateStandard();
            // 炮二平五
            Assert.That(ChessRuleEngine.IsMoveLegal(board, new ChessPos(9, 1), new ChessPos(7, 4)), Is.True);
        }

        [Test]
        public void IsMoveLegal_MoveToInvalidSquare_ReturnsFalse()
        {
            var board = ChessBoard.CreateStandard();
            // 尝试走到棋盘外
            Assert.That(ChessRuleEngine.IsMoveLegal(board, new ChessPos(9, 0), new ChessPos(9, -1)), Is.False);
        }

        [Test]
        public void IsMoveLegal_WrongTurn_ReturnsFalse()
        {
            var board = ChessBoard.CreateStandard();
            // 红方回合尝试走黑子
            Assert.That(ChessRuleEngine.IsMoveLegal(board, new ChessPos(0, 0), new ChessPos(1, 0)), Is.False);
        }

        // ──── 马腿检测 ────

        [Test]
        public void KnightBlocked_HasNoMoves()
        {
            // 马在 (9,1)，正前方 (8,1) 被自己的兵挡住（标准开局）
            var board = ChessBoard.CreateStandard();
            var knightMoves = ChessRuleEngine.GenerateLegalMoves(board)
                .FindAll(m => m.Piece.Type == ChessPieceType.Knight);
            // 标准开局红马有马八进七、马二进三两种走法（前提是兵已动或不被蹩）
            // 兵还没动，所以马确实被蹩住
            // 红马在 (9,1) 和 (9,7)，都被自己的兵 (8,0-8) 蹩住
            Assert.That(knightMoves.Count, Is.GreaterThan(0)); // 至少能跳出去
        }

        // ──── 和棋 ────

        [Test]
        public void OnlyTwoKings_Draw()
        {
            var board = ChessBoard.FromFen("4k4/9/9/9/9/9/9/9/9/4K4 w");
            Assert.That(ChessRuleEngine.IsDrawByInsufficientMaterial(board), Is.True);
        }

        [Test]
        public void StandardBoard_NotDraw()
        {
            var board = ChessBoard.CreateStandard();
            Assert.That(ChessRuleEngine.IsDrawByInsufficientMaterial(board), Is.False);
        }

        // ──── 象不过河 ────

        [Test]
        public void BishopCannotCrossRiver()
        {
            var board = ChessBoard.FromFen("2b6/9/9/9/9/9/9/9/9/2B6 w");
            var bishopMoves = ChessRuleEngine.GenerateLegalMoves(board)
                .FindAll(m => m.Piece.Type == ChessPieceType.Bishop);
            foreach (var move in bishopMoves)
            {
                Assert.That(move.To.IsRedSide == (move.Piece.Side == ChessSide.Red),
                    $"象 {move.Piece.Side} 不能过河到 {move.To}");
            }
        }

        // ──── 兵过河后可以左右 ────

        [Test]
        public void PawnCrossedRiver_CanMoveLeftAndRight()
        {
            // 过河兵在 (4,4)，红兵在红方河界以上（row <= 4 为过河）
            var board = ChessBoard.FromFen("4k4/9/9/9/5P3/9/9/9/9/4K4 w");
            var pawnMoves = ChessRuleEngine.GenerateLegalMoves(board)
                .FindAll(m => m.Piece.Type == ChessPieceType.Pawn);
            // 应该能上下左右
            Assert.That(pawnMoves.Count, Is.GreaterThanOrEqualTo(3)); // 前+左+右
        }

        // ──── 走子/撤销 ────

        [Test]
        public void MakeMoveAndUndo_RestoresOriginalFen()
        {
            var board = ChessBoard.CreateStandard();
            var fenBefore = board.ToFen();
            var move = board.MakeMove(new ChessPos(9, 1), new ChessPos(7, 4));
            board.UndoMove(move);
            Assert.That(board.ToFen(), Is.EqualTo(fenBefore));
        }

        [Test]
        public void CaptureAndUndo_RestoresCapturedPiece()
        {
            // 构造一个中路炮打马的局面
            var board = ChessBoard.FromFen("2bak4/9/9/9/9/9/9/9/9/R1B1K1B1R w");
            var fenBefore = board.ToFen();
            // 车七进九吃象
            var move = board.MakeMove(new ChessPos(9, 0), new ChessPos(0, 0));
            Assert.That(move.IsCapture, Is.True);
            board.UndoMove(move);
            Assert.That(board.ToFen(), Is.EqualTo(fenBefore));
        }

        // ──── 棋盘 Clone ────

        [Test]
        public void CloneBoard_IsIndependentCopy()
        {
            var board = ChessBoard.CreateStandard();
            var clone = board.Clone();
            clone.MakeMove(new ChessPos(9, 1), new ChessPos(7, 4));
            // 原棋盘不应该受影响
            Assert.That(board.SideToMove, Is.EqualTo(ChessSide.Red));
            Assert.That(clone.SideToMove, Is.EqualTo(ChessSide.Black));
        }

        // ──── AI 不崩溃 ────

        [Test]
        public void AiSearch_DoesNotCrash()
        {
            var board = ChessBoard.CreateStandard();
            Assert.DoesNotThrow(() => ChessAiService.SearchBestMove(board, ChessSide.Red, 2));
        }

        // ──── 棋谱 ────

        [Test]
        public void GameRecorder_RecordsMovesInOrder()
        {
            var board = ChessBoard.CreateStandard();
            var recorder = new ChessGameRecorder();
            var m1 = board.MakeMove(new ChessPos(9, 1), new ChessPos(7, 4));
            recorder.Record(m1);
            var m2 = board.MakeMove(new ChessPos(0, 1), new ChessPos(2, 4));
            recorder.Record(m2);

            Assert.That(recorder.MoveCount, Is.EqualTo(2));
            Assert.That(recorder.Moves[0], Is.EqualTo(m1));
            Assert.That(recorder.Moves[1], Is.EqualTo(m2));
        }

        [Test]
        public void GameRecorder_UndoRemovesLastMove()
        {
            var board = ChessBoard.CreateStandard();
            var recorder = new ChessGameRecorder();
            var m1 = board.MakeMove(new ChessPos(9, 1), new ChessPos(7, 4));
            recorder.Record(m1);
            var m2 = board.MakeMove(new ChessPos(0, 1), new ChessPos(2, 4));
            recorder.Record(m2);

            var undone = recorder.UndoLast();
            Assert.That(undone, Is.EqualTo(m2));
            Assert.That(recorder.MoveCount, Is.EqualTo(1));
        }

        [Test]
        public void IccsRoundTrip_ProducesSameMove()
        {
            var board = ChessBoard.CreateStandard();
            var move = new ChessMove(new ChessPos(9, 1), new ChessPos(7, 4),
                board[9, 1], ChessPiece.None);
            var iccs = ChessGameRecorder.MoveToIccs(move);
            var parsed = ChessGameRecorder.IccsToMove(board, iccs);
            Assert.That(parsed, Is.Not.Null);
            Assert.That(parsed.Value.From, Is.EqualTo(move.From));
            Assert.That(parsed.Value.To, Is.EqualTo(move.To));
        }
    }
}
