using System;

namespace Assets.Utils
{
    public class ActionDisposer: IDisposable
    {
        private bool Disposed;
        private readonly Action Action;

        public ActionDisposer(Action action)
        {
            Action = action;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Action();
            Disposed = true;
        }
    }
}