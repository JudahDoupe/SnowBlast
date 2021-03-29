#nullable enable
using System;
using JetBrains.Annotations;

namespace Assets.Utils.ProceduralAnimationLibrary.Cutscenes
{
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class CutsceneVerbAttribute : Attribute
    {
        public readonly string? Name;

        public CutsceneVerbAttribute(string? verb = null)
        {
            Name = verb;
        }
    }
}