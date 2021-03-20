#nullable enable
using System;

namespace Assets.Utils.JBehavior
{
    public static class JBehaviorSetExtensions
    {
        public static T Then<T>(this T self, float duration, Action<float> callback, float[]? curve = null)
            where T: JBehaviorSet
        {
            self.Append(new JAnimationBehavior(duration, callback, curve ?? JCurve.Linear));
            return self;
        }

        public static TSelf Computed<TSelf, TState>(this TSelf self, Func<TState> initial, Func<TState, float> duration, Action<TState, float> callback, float[]? curve = null)
            where TSelf : JBehaviorSet
        {
            self.Append(new JComputedBehavior<TState>(initial, duration, callback, curve ?? JCurve.Linear));
            return self;
        }

        public static T Then<T>(this T self, Action callback)
            where T : JBehaviorSet
        {
            self.Append(new JActionBehavior(callback));
            return self;
        }
        public static T Wait<T>(this T self, float duration)
            where T : JBehaviorSet
        {
            self.Then(duration, _ => { });
            return self;
        }
    }
}