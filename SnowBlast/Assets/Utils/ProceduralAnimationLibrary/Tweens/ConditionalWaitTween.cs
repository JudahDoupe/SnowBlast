using System;
using System.Collections;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweens
{
    public class ConditionalWaitTween : IEnumeratorTween
    {
        private readonly Func<bool> Condition;

        public ConditionalWaitTween(Func<bool> condition)
        {
            Condition = condition;
        }

        public IEnumerator Start()
        {
            while (Condition())
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}