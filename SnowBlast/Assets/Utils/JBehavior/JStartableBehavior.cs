#nullable enable
using System;
using FluentAssertions;
using UnityEngine;

namespace Assets.Utils.JBehavior
{
    public class JStartableBehavior : JBehaviorSet
    {
        private readonly MonoBehaviour GameObject;

        public JStartableBehavior(MonoBehaviour gameObject)
        {
            GameObject = gameObject;
        }

        public void Start(Action? onComplete = null)
        {
            GameObject.Should().NotBeNull();
            GameObject.StartCoroutine(Begin(onComplete));
        }
    }
}