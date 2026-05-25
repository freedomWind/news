using System;
using System.Collections.Generic;

namespace NewsFramework.Simulation
{
    [Serializable]
    public sealed class GameCommandData
    {
        public int tick;
        public int sequence;
        public string playerId;
        public string type;
        public string payload;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public string GetParameter(string key)
        {
            if (string.IsNullOrEmpty(key) || parameters == null)
            {
                return string.Empty;
            }

            return parameters.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
