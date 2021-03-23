using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using FluentAssertions;

#nullable  enable
namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public class SerialTweener: ITweener, IEnumeratorTween
    {
        private readonly List<ITween> Behaviors = new List<ITween>();

        public void Append(ITween behavior)
        {
            Behaviors.Add(behavior);
        }

        public void Append(SerialTweener other)
        {
            Behaviors.AddRange(other.Behaviors);
        }

        public IEnumerator Begin(Action? onComplete = null)
        {
            var remainder = RunInitialActions();
            if (onComplete is { })
            {
                remainder.Add(new ActionTween(onComplete));
            }
            return StartEnumeration(remainder);
        }

        private List<ITween> RunInitialActions()
        {
            var remainder = Behaviors.SkipWhile(b =>
            {
                if (b is ActionTween action)
                {
                    action.Act();
                    return true;
                }

                return false;
            });
            return remainder.ToList();
        }

        private IEnumerator StartEnumeration(IEnumerable<ITween> jBehaviors)
        {
            foreach (var behavior in jBehaviors)
            {
                if (behavior is ActionTween action)
                {
                    action.Act();
                }
                else if (behavior is IEnumeratorTween enumerator)
                {
                    var e = enumerator.Start();
                    while (e.MoveNext()) yield return e.Current;
                }
            }
        }

        public IEnumerator Start()
        {
            return Begin();
        }
    }
}