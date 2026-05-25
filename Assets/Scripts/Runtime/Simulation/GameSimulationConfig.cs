using System;

namespace NewsFramework.Simulation
{
    [Serializable]
    public sealed class GameSimulationConfig
    {
        public string gameId = "xiangqi";
        public int tickRate = 20;
        public bool deterministic = true;
        public string initialState;
        public string initialStateFormat = "fen";

        public float TickDeltaSeconds => tickRate <= 0 ? 0.05f : 1f / tickRate;
    }
}
