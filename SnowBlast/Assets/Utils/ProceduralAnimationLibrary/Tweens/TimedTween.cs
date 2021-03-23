using System;
using System.Collections;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public class TimedTween : IEnumeratorTween
    {
        private readonly JComputedBehavior<int> Implementation;

        public TimedTween(float duration, Action<float> callback, float[] curve)
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
}