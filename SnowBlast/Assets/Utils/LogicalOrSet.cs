using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Utils
{
    public class LogicalOrSet: INotifier<bool>
    {
        public bool IsTrue => Blockers.Any();

        private readonly HashSet<object> Blockers = new HashSet<object>();
        private readonly Notifier<bool> NotifierImplementation = new Notifier<bool>();

        public void Add(object blocker)
        {
            Blockers.Add(blocker);
            if (Blockers.Count == 1)
            {
                NotifierImplementation.Notify(true);
            }
        }

        public void Remove(object blocker)
        {
            Blockers.Remove(blocker);
            if (Blockers.Count == 0)
            {
                NotifierImplementation.Notify(false);
            }
        }

        public IDisposable Subscribe(Action<bool> subscriber)
        {
            return NotifierImplementation.Subscribe(subscriber);
        }
    }
}