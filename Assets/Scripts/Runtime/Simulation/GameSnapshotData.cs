using System;
using System.Collections.Generic;

namespace NewsFramework.Simulation
{
    [Serializable]
    public sealed class GameSnapshotData
    {
        public string gameId;
        public int tick;
        public string stateFormat;
        public string state;
        public Dictionary<string, string> metadata = new Dictionary<string, string>();
    }
}
