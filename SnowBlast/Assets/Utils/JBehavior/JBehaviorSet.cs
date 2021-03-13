using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable  enable
namespace Assets.Utils.JBehavior
{
    public class JBehaviorSet
    {

        public bool InProgress = false;
        private readonly IReadOnlyList<IJBehavior> Behaviors;

        private JBehaviorSet(IEnumerable<IJBehavior> actions)
        {
            Behaviors = actions.ToList();
        }

        private JBehaviorSet(IJBehavior action)
        {
            Behaviors = new[] {action};
        }

        public static JBehaviorSet Animate(float duration, Action<float> callback, float[]? curve = null)
        {
            return new JBehaviorSet(new JAnimationBehavior(duration, callback, curve ?? JCurve.Linear));
        }

        public static JBehaviorSet Animate(Action callback)
        {
            return new JBehaviorSet(new JActionBehavior(callback));
        }

        public JBehaviorSet Then(float duration, Action<float> callback, float[]? curve = null)
        {
            return Append(new JAnimationBehavior(duration, callback, curve ?? JCurve.Linear));
        }

        public JBehaviorSet Then(Action callback)
        {
            return Append(new JActionBehavior(callback));
        }

        private JBehaviorSet Prewarm()
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
            return new JBehaviorSet(remainder);
        }

        public IEnumerator Start()
        {
            return Prewarm().StartEnumeration();
        }

        private IEnumerator StartEnumeration()
        {
            InProgress = true;

            foreach (var behavior in Behaviors)
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

        private JBehaviorSet Append(IJBehavior behavior)
        {
            return new JBehaviorSet(Behaviors.Append(behavior));
        }
    }
}