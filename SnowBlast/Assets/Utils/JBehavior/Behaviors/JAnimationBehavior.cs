using System;
using System.Collections;

namespace Assets.Utils.JBehavior.Behaviors
{
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
}