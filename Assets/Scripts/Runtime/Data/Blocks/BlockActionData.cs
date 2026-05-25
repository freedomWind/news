using System;
using System.Collections.Generic;

namespace NewsFramework.Data.Blocks
{
    [Serializable]
    public sealed class BlockActionData
    {
        public string type;
        public string target;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();

        public string GetParameter(string key)
        {
            if (string.IsNullOrEmpty(key) || parameters == null)
            {
                return string.Empty;
            }

            return parameters.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public static BlockActionData None()
        {
            return new BlockActionData
            {
                type = "none",
                target = string.Empty
            };
        }
    }
}
