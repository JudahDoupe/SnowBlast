#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public class ParallelTweener: ITweener, IEnumeratorTween
    {
        private readonly List<ITween> Behaviors = new List<ITween>();

        public void AppendInternal(ITween tween)
        {
            Behaviors.Add(tween);
        }

        public IEnumerator Begin(Action? onCompleteCallback = null)
        {
            var remainder = RunInitialActions();
            if (onCompleteCallback is { })
            {
                remainder.Add(new ActionTween(onCompleteCallback));
            }
            return StartEnumeration(remainder);
        }

        public void Instant()
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Instant();
            }
        }

        private IEnumerator StartEnumeration(IEnumerable<ITween> jBehaviors)
        {
            var enumerators = new List<IEnumerator>();
            foreach (var behavior in jBehaviors)
            {
                if (behavior is ActionTween action)
                {
                    action.Instant();
                }
                else if (behavior is IEnumeratorTween enumerator)
                {
                    enumerators.Add(enumerator.Start());
                }
            }

            while (enumerators.Any())
            {
                var copy = enumerators.ToList();
                enumerators = new List<IEnumerator>();
                var any = false;
                foreach (var e in copy)
                {
                    if (e.MoveNext())
                    {
                        enumerators.Add(e);
                        any = true;
                    }
                }
                if (any) yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator Start()
        {
            return Begin();
        }

        private List<ITween> RunInitialActions()
        {
            var remainder = Behaviors.SkipWhile(b =>
            {
                if (b is ActionTween action)
                {
                    action.Instant();
                    return true;
                }

                return false;
            });
            return remainder.ToList();
        }
    }
}