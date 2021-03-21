using System;
using System.Collections;
using UnityEngine;

namespace Assets.Utils.JBehavior.Behaviors
{
    public class JWaitBehavior : IJEnumeratorBehavior
    {
        private readonly Func<bool> Condition;

        public JWaitBehavior(Func<bool> condition)
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