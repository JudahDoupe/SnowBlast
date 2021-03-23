#nullable enable
using System;
using System.Collections.Generic;
using Assets.Utils.ProceduralAnimationLibrary.Tweens;
using UnityEngine;

namespace Assets.Utils.ProceduralAnimationLibrary.Cutscenes
{
    public class VerbDetails
    {
        public readonly Func<GameObject, object, Dictionary<string, string>, ITween> Action;
        public readonly bool DirectObjectIsString;
        public readonly bool NoSubject;

        public VerbDetails(Func<GameObject, object, Dictionary<string, string>, ITween> action, bool directObjectIsString,
            bool noSubject)
        {
            Action = action;
            DirectObjectIsString = directObjectIsString;
            NoSubject = noSubject;
        }
    }
}