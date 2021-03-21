using System;

namespace Assets.Utils.JBehavior.Behaviors
{
    public class JActionBehavior : IJBehavior
    {
        private readonly Action Callback;

        public JActionBehavior(Action callback)
        {
            Callback = callback;
        }

        public void Act() => Callback();
    }
}