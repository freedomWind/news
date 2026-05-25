using System;
using System.Collections.Generic;



namespace NewsFramework.Simulation
{
    public sealed class NullGameSimulation : IGameSimulation
    {
        private readonly Queue<GameCommandData> pendingCommands = new Queue<GameCommandData>();
        private GameSimulationConfig config;
        private GameSnapshotData snapshot;

        public string SimulationId { get; private set; } = Guid.NewGuid().ToString("N");
        public string GameId => config != null ? config.gameId : string.Empty;
        public int CurrentTick { get; private set; }
        public bool IsLoaded { get; private set; }

        public void Load(GameSimulationConfig config)
        {
            this.config = config ?? new GameSimulationConfig();
            CurrentTick = 0;
            snapshot = new GameSnapshotData
            {
                gameId = this.config.gameId,
                tick = CurrentTick,
                stateFormat = this.config.initialStateFormat,
                state = this.config.initialState
            };
            IsLoaded = true;
        }

        public void EnqueueCommand(GameCommandData command)
        {
            if (command != null)
            {
                pendingCommands.Enqueue(command);
            }
        }

        public GameTickResult Tick(GameTickInput input)
        {
            if (!IsLoaded)
            {
                return new GameTickResult
                {
                    tick = CurrentTick,
                    success = false,
                    error = "Simulation is not loaded."
                };
            }

            CurrentTick = input != null ? input.tick : CurrentTick + 1;
            DrainCommands(input);

            snapshot = CreateSnapshot();
            return new GameTickResult
            {
                tick = CurrentTick,
                snapshot = snapshot
            };
        }

        public void ApplySnapshot(GameSnapshotData snapshot)
        {
            this.snapshot = snapshot;
            CurrentTick = snapshot != null ? snapshot.tick : 0;
            IsLoaded = snapshot != null;
        }

        public GameSnapshotData CreateSnapshot()
        {
            return new GameSnapshotData
            {
                gameId = GameId,
                tick = CurrentTick,
                stateFormat = snapshot != null ? snapshot.stateFormat : string.Empty,
                state = snapshot != null ? snapshot.state : string.Empty
            };
        }

        public void Reset()
        {
            pendingCommands.Clear();
            CurrentTick = 0;
            IsLoaded = false;
            snapshot = null;
        }

        public void Dispose()
        {
            Reset();
        }

        private void DrainCommands(GameTickInput input)
        {
            if (input != null && input.commands != null)
            {
                foreach (var command in input.commands)
                {
                    EnqueueCommand(command);
                }
            }

            while (pendingCommands.Count > 0)
            {
                pendingCommands.Dequeue();
            }
        }
    }
}
