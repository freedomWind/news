using System.Collections.Generic;


namespace NewsFramework.Simulation
{
    public sealed class GameTickResult
    {
        public int tick;
        public bool success = true;
        public string error;
        public GameSnapshotData snapshot;
        public List<GameSimulationEvent> events = new List<GameSimulationEvent>();
    }
}
