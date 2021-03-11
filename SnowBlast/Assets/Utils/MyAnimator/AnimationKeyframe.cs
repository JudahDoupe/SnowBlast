using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Utils.MyAnimator
{
    public class AnimationKeyframe
    {
        private readonly float Duration;
        private List<IAnimationProperty> Fields;
        public bool InProgress = false;

        public AnimationKeyframe(float duration)
        {
            Duration = duration;
        }

        public void Add(IAnimationProperty field)
        {
            Fields.Add(field);
        }

        public IEnumerator Start()
        {
            InProgress = true;

            var startTime = Time.fixedTime;
            var timeout = startTime + Duration;

            while (true)
            {
                var ratio = (Time.fixedTime - startTime) / Duration;
                foreach (var item in Fields)
                {
                    item.Increment(ratio);
                }

                if (Time.fixedTime >= timeout)
                {
                    break;
                }
                yield return new WaitForSeconds(Math.Min(0.1f, timeout - Time.fixedTime));
            }

            InProgress = false;
        }
    }
}