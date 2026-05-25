using System;

namespace NewsFramework.Core
{
    public interface IFrameBuffer<T>
    {
        void AddFrameData(int frameIndex, T[] data);
        bool TryGetFrameData(int frameIndex, out T[] data);
        void CleanupExpiredData(int currentFrameIndex, int keepFrames);
        FrameBufferStatus GetStatus();
    }

    [Serializable]
    public sealed class FrameBufferStatus
    {
        public int TotalFrames;
        public int PendingFrames;
        public int ProcessedFrames;
        public int DroppedFrames;

        public void Reset()
        {
            TotalFrames = 0;
            PendingFrames = 0;
            ProcessedFrames = 0;
            DroppedFrames = 0;
        }
    }
}
