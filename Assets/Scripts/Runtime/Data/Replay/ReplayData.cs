using System;
using System.Collections.Generic;

namespace NewsFramework.Data.Replay
{
    [Serializable]
    public sealed class ReplayData
    {
        public string replayId;
        public string gameId = "xiangqi";
        public string initialState;
        public string initialStateFormat = "fen";
        public int startStepIndex;
        public int endStepIndex = -1;
        public bool autoPlay;
        public bool loop;
        public float secondsPerStep = 0.6f;
        public ReplayRenderProfileData renderProfile = new ReplayRenderProfileData();
        public List<ReplayStepData> steps = new List<ReplayStepData>();

        public int ResolveEndStepIndex()
        {
            if (endStepIndex >= 0)
            {
                return endStepIndex;
            }

            return steps == null || steps.Count == 0 ? startStepIndex : steps[steps.Count - 1].index;
        }
    }
}
