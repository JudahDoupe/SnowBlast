using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Utils
{
    public interface INotifier<T>
    {
        IDisposable Subscribe(Action<T> subscriber);
    }

    public class Notifier<T> : INotifier<T>, IDisposable
    {
        private bool Disposed;
        private readonly List<Action<T>> Subscribers = new List<Action<T>>();

        public void Notify(T evt)
        {
            // Note: ToList() Subscribers so we don't throw an exception if it is modified while iterating
            foreach (var subscriber in Subscribers.ToList())
            {
                try
                {
                    subscriber(evt);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public IDisposable Subscribe(Action<T> subscriber)
        {
            if (Disposed) throw new ApplicationException("Attempting to subscribe to disposed notifier.");
            Subscribers.Add(subscriber);
            return new ActionDisposer(() =>
            {
                if (!Disposed) Subscribers.Remove(subscriber);
            });
        }

        public void Dispose()
        {
            if (Disposed) return;
            Subscribers.Clear();
            Disposed = true;
        }

        public void Clear()
        {
            Subscribers.Clear();
        }
    }
}