using System;
using NewsFramework.Simulation.Chess;


namespace NewsFramework.Simulation
{
    /// <summary>中国象棋 IGameSimulation 实现。用 ChessRuleEngine 驱动 tick。</summary>
    public sealed class ChessSimulation : IGameSimulation
    {
        private ChessBoard _board;
        private ChessGameRecorder _recorder;
        private GameSimulationConfig _config;
        private readonly string _simulationId = Guid.NewGuid().ToString("N");

        public string SimulationId => _simulationId;
        public string GameId => _config?.gameId ?? "xiangqi";
        public int CurrentTick { get; private set; }

        public bool IsLoaded => _board != null;

        public void Load(GameSimulationConfig config)
        {
            _config = config ?? new GameSimulationConfig();
            CurrentTick = 0;
            _board = (!string.IsNullOrEmpty(_config.initialState) && _config.initialStateFormat == "fen")
                ? ChessBoard.FromFen(_config.initialState)
                : ChessBoard.CreateStandard();
            _recorder = new ChessGameRecorder();
        }

        public void EnqueueCommand(GameCommandData command) { }

        public GameTickResult Tick(GameTickInput input)
        {
            if (!IsLoaded)
                return new GameTickResult { tick = CurrentTick, success = false, error = "Simulation is not loaded." };

            var events = new System.Collections.Generic.List<GameSimulationEvent>();
            if (input?.commands != null)
            {
                foreach (var cmd in input.commands)
                {
                    if (TryApplyCommand(cmd, out var move, out var error))
                    {
                        _recorder.Record(move);
                        events.Add(new GameSimulationEvent
                        {
                            tick = CurrentTick, type = "move",
                            payload = ChessGameRecorder.MoveToIccs(move),
                            parameters =
                            {
                                { "fromRow", move.From.Row.ToString() }, { "fromCol", move.From.Col.ToString() },
                                { "toRow", move.To.Row.ToString() }, { "toCol", move.To.Col.ToString() },
                                { "piece", move.Piece.ToChar().ToString() }
                            }
                        });
                    }
                    else
                    {
                        events.Add(new GameSimulationEvent { tick = CurrentTick, type = "error", payload = error });
                    }
                }
            }

            CurrentTick = input?.tick ?? CurrentTick + 1;

            if (ChessRuleEngine.IsCheckmate(_board, _board.SideToMove))
            {
                var winner = _board.SideToMove == ChessSide.Red ? ChessSide.Black : ChessSide.Red;
                events.Add(new GameSimulationEvent { tick = CurrentTick, type = "game_over", payload = winner == ChessSide.Red ? "red_win" : "black_win" });
            }
            else if (ChessRuleEngine.IsDrawByInsufficientMaterial(_board))
            {
                events.Add(new GameSimulationEvent { tick = CurrentTick, type = "game_over", payload = "draw" });
            }

            return new GameTickResult { tick = CurrentTick, success = true, snapshot = CreateSnapshot(), events = events };
        }

        public void ApplySnapshot(GameSnapshotData snapshot)
        {
            if (snapshot == null) return;
            try
            {
                _board = ChessBoard.FromFen(snapshot.state);
                CurrentTick = snapshot.tick;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[ChessSimulation] ApplySnapshot failed at tick {snapshot.tick}: {ex.Message}");
            }
        }

        public GameSnapshotData CreateSnapshot() => new GameSnapshotData
        {
            gameId = GameId, tick = CurrentTick, stateFormat = "fen",
            state = _board?.ToFen() ?? string.Empty,
            metadata =
            {
                { "sideToMove", (_board?.SideToMove ?? ChessSide.Red).ToString() },
                { "moveCount", (_recorder?.MoveCount ?? 0).ToString() }
            }
        };

        public void Reset() { _board = null; _recorder = null; CurrentTick = 0; }
        public void Dispose() { Reset(); }

        // ──── GameRoom 对局接口 ────

        public ChessBoard GetBoard() => _board;
        public ChessGameRecorder GetRecorder() => _recorder;
        public ChessSide CurrentSide => _board?.SideToMove ?? ChessSide.Red;

        public bool TryMakeMove(ChessPos from, ChessPos to, out ChessMove move, out string error)
        {
            move = default; error = null;
            if (!IsLoaded) { error = "Simulation is not loaded."; return false; }
            var piece = _board.GetPiece(from);
            if (!piece.IsValid) { error = $"No piece at {from}"; return false; }
            if (piece.Side != _board.SideToMove) { error = $"It's {_board.SideToMove}'s turn"; return false; }
            if (!ChessRuleEngine.IsMoveLegal(_board, from, to)) { error = "Illegal move"; return false; }
            move = _board.MakeMove(from, to);
            _recorder.Record(move);
            CurrentTick++;
            return true;
        }

        public bool UndoLastMove(out ChessMove undone)
        {
            undone = default;
            if (_recorder == null || _recorder.MoveCount == 0) return false;
            var last = _recorder.UndoLast();
            if (!last.HasValue) return false;
            _board.UndoMove(last.Value);
            CurrentTick = Math.Max(0, CurrentTick - 1);
            undone = last.Value;
            return true;
        }

        private bool TryApplyCommand(GameCommandData cmd, out ChessMove move, out string error)
        {
            move = default; error = null;
            if (cmd.type != "move") { error = $"Unknown command type: {cmd.type}"; return false; }

            var iccs = cmd.payload;
            if (!string.IsNullOrEmpty(iccs) && iccs.Length >= 4)
            {
                var parsed = ChessGameRecorder.IccsToMove(_board, iccs);
                if (parsed.HasValue)
                    return TryMakeMove(parsed.Value.From, parsed.Value.To, out move, out error);
            }

            if (cmd.parameters.TryGetValue("fromRow", out var frs) && cmd.parameters.TryGetValue("fromCol", out var fcs)
                && cmd.parameters.TryGetValue("toRow", out var trs) && cmd.parameters.TryGetValue("toCol", out var tcs))
            {
                return TryMakeMove(
                    new ChessPos(int.Parse(frs), int.Parse(fcs)),
                    new ChessPos(int.Parse(trs), int.Parse(tcs)),
                    out move, out error);
            }

            error = "Invalid move command format.";
            return false;
        }
    }
}
