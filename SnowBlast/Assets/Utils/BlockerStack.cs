using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Utils
{
    public class BlockerStack: INotifier<BlockerStack.BlockState>
    {
        public bool IsBlocked => Blockers.Any();

        private readonly HashSet<object> Blockers = new HashSet<object>();
        private readonly Notifier<BlockState> NotifierImplementation = new Notifier<BlockState>();

        public void Block(object blocker)
        {
            Blockers.Add(blocker);
            if (Blockers.Count == 1)
            {
                NotifierImplementation.Notify(BlockState.Blocked);
            }
        }

        public void Unblock(object blocker)
        {
            Blockers.Remove(blocker);
            if (Blockers.Count == 0)
            {
                NotifierImplementation.Notify(BlockState.Unblocked);
            }
        }

        public enum BlockState
        {
            Unblocked,
            Blocked
        }

        public IDisposable Subscribe(Action<BlockState> subscriber)
        {
            return NotifierImplementation.Subscribe(subscriber);
        }
    }
}