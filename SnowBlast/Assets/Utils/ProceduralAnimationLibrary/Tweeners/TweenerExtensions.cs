#nullable enable
using System;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public static class TweenerExtensions
    {
        public static T Then<T>(this T self, float duration, Action<float> callback, float[]? curve = null)
            where T: ITweener
        {
            self.Append(new TimedTween(duration, callback, curve ?? JCurve.Linear));
            return self;
        }

        public static T Then<T>(this T self, float duration, Vector3 start, Vector3 end, Action<Vector3> callback,
            float[]? curve = null) where T : ITweener
        {
            return self.Then(duration, r =>
            {
                var n = Vector3.Lerp(start, end, r);
                callback(n);
            }, curve);
        }

        public static TSelf Computed<TSelf, TState>(this TSelf self, Func<TState> initial, Func<TState, float> duration, Action<TState, float> callback, float[]? curve = null)
            where TSelf : ITweener
        {
            self.Append(new JComputedBehavior<TState>(initial, duration, callback, curve ?? JCurve.Linear));
            return self;
        }

        public static T Then<T>(this T self, Action callback)
            where T : ITweener
        {
            self.Append(new ActionTween(callback));
            return self;
        }
        public static T Wait<T>(this T self, float duration)
            where T : ITweener
        {
            self.Then(duration, _ => { });
            return self;
        }

        public static T While<T>(this T self, Func<bool> condition)
            where T : ITweener
        {
            self.Append(new ConditionalWaitTween(condition));
            return self;
        }
    }
}