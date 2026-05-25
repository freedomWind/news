using System;



namespace NewsFramework.Simulation
{
    public interface IGameSimulation : IDisposable
    {
        string SimulationId { get; }
        string GameId { get; }
        int CurrentTick { get; }
        bool IsLoaded { get; }

        void Load(GameSimulationConfig config);
        void EnqueueCommand(GameCommandData command);
        GameTickResult Tick(GameTickInput input);
        void ApplySnapshot(GameSnapshotData snapshot);
        GameSnapshotData CreateSnapshot();
        void Reset();
    }
}
