using System;

namespace NewsFramework.Data.Replay
{
    [Serializable]
    public sealed class ReplayStepData
    {
        public int index;
        public string command;
        public string notation;
        public string comment;
        public float duration = -1f;
    }
}
