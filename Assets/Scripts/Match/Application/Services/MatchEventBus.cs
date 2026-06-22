using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class MatchEventBus : IMatchEventBus
    {
        private readonly Dictionary<Type, List<object>> _observers;
        public MatchEventBus()
        {
            _observers = new();
        }

        public void Subscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
        {
            var type = typeof(T);
            if(!_observers.TryGetValue(type, out var list))
            {
                list = new List<object>();
                _observers[type] = list;
            }
            list.Add(observer);
        }

        public void Unsubscribe<T>(IMatchObserver<T> observer) where T : IMatchEvent
        {
            var type = typeof(T);
            if(!_observers.TryGetValue(type, out var list)) return;
            list.Remove(observer);
        }

        public void DispatchEvent<T>(T e) where T : IMatchEvent
        {
            var type = typeof(T);
            if(!_observers.TryGetValue(type, out var list)) return;

            foreach (var observer in list)
            {
                ((IMatchObserver<T>)observer).Notify(e);
            }
        }
    }
}