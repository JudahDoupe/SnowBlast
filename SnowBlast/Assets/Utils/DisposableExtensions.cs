using System;

namespace Assets.Utils
{
    public static class DisposableExtensions
    {
        public static IDisposable Then(this IDisposable self, IDisposable next)
        {
            return new ActionDisposer(() =>
            {
                self.Dispose();
                next.Dispose();
            });
        }
    }
}