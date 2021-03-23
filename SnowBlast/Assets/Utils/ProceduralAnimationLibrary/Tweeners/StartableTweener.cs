#nullable enable
using System;
using System.Collections;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using FluentAssertions;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public class StartableTweener: ITweener
    {
        private readonly ITweener Implementation;
        private readonly MonoBehaviour GameObject;

        public StartableTweener(ITweener implementation, MonoBehaviour gameObject)
        {
            Implementation = implementation;
            GameObject = gameObject;
        }

        public Coroutine Start(Action? onComplete = null)
        {
            GameObject.Should().NotBeNull();
            return GameObject.StartCoroutine(Begin(onComplete));
        }

        public void Append(ITween tween)
        {
            Implementation.Append(tween);
        }

        public IEnumerator Begin(Action? onCompleteCallback)
        {
            return Implementation.Begin(onCompleteCallback);
        }

        public bool InProgress => Implementation.InProgress;
    }
}