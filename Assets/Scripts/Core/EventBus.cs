using System;
using System.Collections.Generic;

namespace NewsFramework.Core
{
    public sealed class EventBus
    {
        private readonly Dictionary<Type, Delegate> _typedHandlers = new Dictionary<Type, Delegate>();
        private Action _simpleHandlers;

        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var type = typeof(T);
            _typedHandlers[type] = Delegate.Combine(
                _typedHandlers.TryGetValue(type, out var existing) ? existing : null,
                handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var type = typeof(T);
            if (_typedHandlers.TryGetValue(type, out var existing))
            {
                var removed = Delegate.Remove(existing, handler);
                if (removed == null) _typedHandlers.Remove(type);
                else _typedHandlers[type] = removed;
            }
        }

        public void Fire<T>(T args)
        {
            var type = typeof(T);
            if (_typedHandlers.TryGetValue(type, out var existing) && existing is Action<T> action)
            {
                action.Invoke(args);
            }
        }

        public void Subscribe(Action handler)
        {
            _simpleHandlers += handler;
        }

        public void Unsubscribe(Action handler)
        {
            _simpleHandlers -= handler;
        }

        public void Fire()
        {
            _simpleHandlers?.Invoke();
        }

        public void Clear()
        {
            _typedHandlers.Clear();
            _simpleHandlers = null;
        }
    }
}
