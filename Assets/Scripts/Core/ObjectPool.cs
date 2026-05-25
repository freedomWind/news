using System;
using System.Collections.Concurrent;
using System.Threading;

namespace NewsFramework.Core
{
    public sealed class ObjectPool<T> where T : class
    {
        private readonly ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();
        private readonly Func<T> _factory;
        private readonly Action<T> _resetAction;
        private readonly int _maxSize;

        private int _activeCount;
        private int _totalCount;
        private readonly object _statsLock = new object();

        public ObjectPool(Func<T> factory, Action<T> resetAction = null, int maxSize = 100)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _resetAction = resetAction;
            _maxSize = maxSize > 0 ? maxSize : throw new ArgumentException("maxSize must be positive", nameof(maxSize));
        }

        public T Get()
        {
            if (_pool.TryDequeue(out var item))
            {
                lock (_statsLock) { _activeCount++; }
                _resetAction?.Invoke(item);
                return item;
            }

            lock (_statsLock)
            {
                _activeCount++;
                _totalCount++;
            }

            return _factory();
        }

        public void Return(T item)
        {
            if (item == null) return;

            if (_pool.Count < _maxSize)
            {
                _pool.Enqueue(item);
                lock (_statsLock) { _activeCount--; }
            }
            else
            {
                lock (_statsLock)
                {
                    _activeCount--;
                    _totalCount--;
                }
            }
        }

        public void Clear()
        {
            while (_pool.TryDequeue(out _)) { }
            lock (_statsLock)
            {
                _activeCount = 0;
                _totalCount = 0;
            }
        }

        public ObjectPoolStatus GetStatus()
        {
            lock (_statsLock)
            {
                return new ObjectPoolStatus
                {
                    PooledCount = _pool.Count,
                    ActiveCount = _activeCount,
                    TotalCount = _totalCount,
                    MaxSize = _maxSize
                };
            }
        }
    }

    public sealed class ObjectPoolStatus
    {
        public int PooledCount;
        public int ActiveCount;
        public int TotalCount;
        public int MaxSize;
    }
}
