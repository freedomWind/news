using System;

namespace NewsFramework.Data.Replay
{
    [Serializable]
    public sealed class ReplayRenderProfileData
    {
        public string mode = ReplayRenderModes.Canvas2D;
        public int textureWidth = 1024;
        public int textureHeight = 576;
        public string cameraProfile = "article_preview";
        public string background = "default";
    }
}
