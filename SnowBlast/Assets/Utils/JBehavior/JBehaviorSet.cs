using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public JBehaviorSet()
        {
            Behaviors = new List<IJBehavior>();
        }

        public IEnumerator Start(Action? onComplete = null)
        {
            InProgress = true;
            var remainder = Prewarm().ToList();
            if (onComplete is { })
            {
                remainder.Add(new JActionBehavior(onComplete));
            }
            return StartEnumeration(remainder);
        }

        public JBehaviorSet Then(float duration, Action<float> callback, float[]? curve = null)
        {
            return Append(new JAnimationBehavior(duration, callback, curve ?? JCurve.Linear));
        }

        public JBehaviorSet Computed<T>(Func<T> initial, Func<T, float> duration, Action<T, float> callback, float[]? curve = null)
        {
            return Append(new JComputedBehavior<T>(initial, duration, callback, curve ?? JCurve.Linear));
        }

        public JBehaviorSet Then(Action callback)
        {
            return Append(new JActionBehavior(callback));
        }

        private IEnumerable<IJBehavior> Prewarm()
        {
            InProgress = true;
            var remainder = Behaviors.SkipWhile(b =>
            {
                if (b is JActionBehavior action)
                {
                    action.Act();
                    return true;
                }

                return false;
            });
            return remainder;
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

        private JBehaviorSet Append(IJBehavior behavior)
        {
            return new JBehaviorSet(Behaviors.Append(behavior));
        }

        public JBehaviorSet Append(JBehaviorSet other)
        {
            var newBehaviors = Behaviors.ToList();
            newBehaviors.AddRange(other.Behaviors);
            return new JBehaviorSet(newBehaviors);
        }

        public JBehaviorSet Wait(float duration)
        {
            return Then(duration, _ => { });
        }
    }

    public static class JBehaviorExtensions
    {
        //public static Coroutine StartBehavior(this MonoBehaviour gameObject, JBehaviorSet behaviorSet)
        //{
        //    var items = behaviorSet.Prewarm().ToList();
        //    return gameObject.StartCoroutine(behaviorSet.Start(items));
        //}
    }
}