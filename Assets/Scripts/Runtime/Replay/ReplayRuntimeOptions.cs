using System;
using UnityEngine;

namespace NewsFramework.Replay
{
    [Serializable]
    public sealed class ReplayRuntimeOptions
    {
        public Transform parent;
        public int textureWidth;
        public int textureHeight;
        public bool startHidden = true;

        public static ReplayRuntimeOptions Default(Transform parent)
        {
            return new ReplayRuntimeOptions
            {
                parent = parent
            };
        }
    }
}
