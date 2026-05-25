using System;
using System.Collections.Generic;

namespace NewsFramework.Simulation
{
    [Serializable]
    public sealed class GameSimulationEvent
    {
        public int tick;
        public string type;
        public string payload;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();
    }
}
