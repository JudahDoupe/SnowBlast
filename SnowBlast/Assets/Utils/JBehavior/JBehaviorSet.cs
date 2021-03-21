using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils.JBehavior.Behaviors;
using FluentAssertions;

#nullable  enable
namespace Assets.Utils.JBehavior
{
    public class JBehaviorSet
    {
        public bool InProgress;
        private readonly List<IJBehavior> Behaviors = new List<IJBehavior>();

        public void Append(IJBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        public void Append(JBehaviorSet other)
        {
            Behaviors.AddRange(other.Behaviors);
        }

        public IEnumerator Begin(Action? onComplete = null)
        {
            InProgress.Should().Be(false, "A single behavior set should only be started once.");
            InProgress = true;
            var remainder = RunInitialActions();
            if (onComplete is { })
            {
                remainder.Add(new JActionBehavior(onComplete));
            }
            return StartEnumeration(remainder);
        }

        private List<IJBehavior> RunInitialActions()
        {
            var remainder = Behaviors.SkipWhile(b =>
            {
                if (b is JActionBehavior action)
                {
                    action.Act();
                    return true;
                }

                return false;
            });
            return remainder.ToList();
        }

        private IEnumerator StartEnumeration(IEnumerable<IJBehavior> jBehaviors)
        {
            foreach (var behavior in jBehaviors)
            {
                if (behavior is JActionBehavior action)
                {
                    action.Act();
                }
                else if (behavior is IJEnumeratorBehavior enumerator)
                {
                    var e = enumerator.Start();
                    while (e.MoveNext()) yield return e.Current;
                }
            }

            InProgress = false;
        }
    }
}