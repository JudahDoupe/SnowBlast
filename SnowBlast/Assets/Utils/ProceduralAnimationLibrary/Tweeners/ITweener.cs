#nullable enable
using System;
using System.Collections;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public interface ITweener
    {
        void Append(ITween tween);

        IEnumerator Begin(Action? onCompleteCallback = null);
    }
}