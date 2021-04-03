using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;

#nullable  enable
namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public class SerialTweener: ITweener, IEnumeratorTween
    {
        private readonly List<ITween> Behaviors = new List<ITween>();

        public void Instant()
        {
            foreach (var behavior in Behaviors)
            {
                behavior.Instant();
            }
        }

        public void AppendInternal(ITween behavior)
        {
            Behaviors.Add(behavior);
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
                    action.Instant();
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
                    action.Instant();
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