using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace NewsFramework.Core
{
    public sealed class CircularFrameBuffer<T> : IFrameBuffer<T>
    {
        private readonly ConcurrentDictionary<int, T[]> _frameData;
        private readonly FrameBufferStatus _status;
        private int _minFrameIndex = -1;

        public CircularFrameBuffer(int capacity = 120)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be positive", nameof(capacity));

            _frameData = new ConcurrentDictionary<int, T[]>(Environment.ProcessorCount, capacity);
            _status = new FrameBufferStatus();
        }

        public void AddFrameData(int frameIndex, T[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _frameData.AddOrUpdate(frameIndex,
                _ =>
                {
                    Interlocked.Increment(ref _status.TotalFrames);
                    Interlocked.Increment(ref _status.PendingFrames);
                    return data;
                },
                (_, __) => data);

            int currentMin = _minFrameIndex;
            if (currentMin < 0 || frameIndex < currentMin)
            {
                Interlocked.Exchange(ref _minFrameIndex, frameIndex);
            }
        }

        public bool TryGetFrameData(int frameIndex, out T[] data)
        {
            if (_frameData.TryGetValue(frameIndex, out data))
            {
                Interlocked.Decrement(ref _status.PendingFrames);
                Interlocked.Increment(ref _status.ProcessedFrames);
                return true;
            }

            return false;
        }

        public void CleanupExpiredData(int currentFrameIndex, int keepFrames)
        {
            if (currentFrameIndex < 0 || keepFrames < 0)
                throw new ArgumentException("Invalid parameters for cleanup");

            int cutoffFrame = currentFrameIndex - keepFrames;

            var keysToRemove = _frameData
                .Where(kvp => kvp.Key < cutoffFrame)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                if (_frameData.TryRemove(key, out _))
                {
                    Interlocked.Increment(ref _status.DroppedFrames);
                }
            }

            if (_minFrameIndex < cutoffFrame)
            {
                Interlocked.Exchange(ref _minFrameIndex, cutoffFrame);
            }
        }

        public FrameBufferStatus GetStatus()
        {
            return new FrameBufferStatus
            {
                TotalFrames = _status.TotalFrames,
                PendingFrames = _status.PendingFrames,
                ProcessedFrames = _status.ProcessedFrames,
                DroppedFrames = _status.DroppedFrames
            };
        }
    }
}
