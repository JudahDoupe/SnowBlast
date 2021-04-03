#nullable enable
using System;
using System.Collections;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Tweeners
{
    public interface ITweener
    {
        void AppendInternal(ITween tween);

        IEnumerator Begin(Action? onCompleteCallback = null);
    }
}