using System;
using System.Collections;
using UnityEngine;

namespace Assets.Utils.JBehavior
{
    public interface IJBehavior
    {
    }

    public interface IJEnumeratorBehavior : IJBehavior
    {
        IEnumerator Start();
    }

    public class JAnimationBehavior : IJEnumeratorBehavior
    {
        private readonly JComputedBehavior<int> Implementation;

        public JAnimationBehavior(float duration, Action<float> callback, float[] curve)
        {
            Implementation = new JComputedBehavior<int>(
                () => 0,
                _ => duration,
                (_, r) => callback(r),
                curve
            );
        }

        public IEnumerator Start()
        {
            return Implementation.Start();
        }
    }

    public class JComputedBehavior<T> : IJEnumeratorBehavior
    {
        private readonly Func<T, float> Duration;
        private readonly Action<T, float> Callback;
        private readonly float[] Curve;
        private readonly Func<T> Initial;

        public JComputedBehavior(Func<T> initial, Func<T, float> duration, Action<T, float> callback, float[] curve)
        {
            Curve = curve ?? JCurve.Linear;
            Duration = duration;
            Callback = callback;
            Initial = initial;
        }

        public IEnumerator Start()
        {
            var timeIn = Time.fixedTime;
            var initial = Initial();
            var duration = Duration(initial);
            while (Time.fixedTime < timeIn + duration)
            {
                var ratio = (Time.fixedTime - timeIn) / duration;
                Callback(initial, JCurve.CurveRatio(ratio, Curve));
                yield return new WaitForEndOfFrame();
            }

            Callback(initial, 1.0f);
        }
    }

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