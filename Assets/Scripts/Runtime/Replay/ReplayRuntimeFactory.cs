using NewsFramework.Data.Replay;

namespace NewsFramework.Replay
{
    public static class ReplayRuntimeFactory
    {
        public static IReplayRuntime Create(ReplayData data)
        {
            if (data == null || data.renderProfile == null)
            {
                return new NullReplayRuntime();
            }

            switch (data.renderProfile.mode)
            {
                case ReplayRenderModes.Canvas2D:
                case ReplayRenderModes.Scene2D:
                case ReplayRenderModes.Scene3D:
                default:
                    return new NullReplayRuntime();
            }
        }
    }
}
