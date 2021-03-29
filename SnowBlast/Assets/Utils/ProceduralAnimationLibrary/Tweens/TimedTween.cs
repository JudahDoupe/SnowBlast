using System;
using System.Collections;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public class TimedTween : JComputedBehavior<int>
    {
        public TimedTween(float duration, Action<float> callback, float[] curve) :
            base(() => 0, _ => duration, (_, r) => callback(r), curve)
        {
        }
    }
}