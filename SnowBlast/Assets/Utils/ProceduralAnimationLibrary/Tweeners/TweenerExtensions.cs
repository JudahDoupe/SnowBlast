#nullable enable
using System;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public static class TweenerExtensions
    {
        public static T Append<T>(this T self, ITween item) where T : ITweener
        {
            self.AppendInternal(item);
            return self;
        }

        public static T Then<T>(this T self, float duration, Action<float> callback, float[]? curve = null)
            where T: ITweener
        {
            return self.Append(new TimedTween(duration, callback, curve ?? JCurve.Linear));
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
            return self.Append(new JComputedBehavior<TState>(initial, duration, callback, curve ?? JCurve.Linear));
        }

        public static T Then<T>(this T self, Action callback)
            where T : ITweener
        {
            return self.Append(new ActionTween(callback));
        }
        public static T Wait<T>(this T self, float duration)
            where T : ITweener
        {
            return self.Then(duration, _ => { });
        }

        public static T While<T>(this T self, Func<bool> condition)
            where T : ITweener
        {
            return self.Append(new ConditionalWaitTween(condition));
        }

        public static T FadeIn<T>(this T self, float time, Graphic graphic) where T : ITweener
        {
            return self.Then(time, ratio => graphic.SetAlpha(ratio));
        }

        public static T FadeOut<T>(this T self, float time, Graphic graphic) where T : ITweener
        {
            return self.Then(time, ratio => graphic.SetAlpha(1.0f - ratio));
        }
    }
}