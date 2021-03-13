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
        private readonly float Duration;
        private readonly Action<float> Callback;
        private readonly float[] Curve;

        public JAnimationBehavior(float duration, Action<float> callback, float[] curve)
        {
            Curve = curve ?? JCurve.Linear;
            Duration = duration;
            Callback = callback;
        }

        public IEnumerator Start()
        {
            var timeIn = Time.fixedTime;
            while (Time.fixedTime < timeIn + Duration)
            {
                var ratio = (Time.fixedTime - timeIn) / Duration;
                Callback(JCurve.CurveRatio(ratio, Curve));
                yield return new WaitForEndOfFrame();
            }

            Callback(1.0f);
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