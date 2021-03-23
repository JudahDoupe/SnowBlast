using System;
using System.Collections;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public class JComputedBehavior<T> : IEnumeratorTween
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
                yield return new WaitForFixedUpdate();
            }

            Callback(initial, 1.0f);
        }
    }
}