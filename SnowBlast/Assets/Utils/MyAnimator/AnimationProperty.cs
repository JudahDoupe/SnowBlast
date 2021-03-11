using System;
using UnityEditor.Media;
using UnityEngine;

namespace Assets.Utils.MyAnimator
{
    public interface IAnimationProperty
    {
        void Increment(float ratio);
    }

    public class AnimationProperty: IAnimationProperty
    {
        private readonly float StartValue;
        private readonly float EndValue;
        private readonly Action<float> FrameSet;

        public AnimationProperty(float startValue, float endValue,  Action<float> frameSet)
        {
            StartValue = startValue;
            EndValue = endValue;
            FrameSet = frameSet;
        }

        public void Increment(float ratio)
        {
            FrameSet(StartValue + (EndValue - StartValue) * ratio);
        }
    }

    public class AnimationRotation: IAnimationProperty
    {
        private readonly GameObject GameObject;
        private readonly Vector3 Axis;
        private readonly float Degrees;
        private Quaternion OriginalRotation;

        public AnimationRotation(GameObject gameObject, Vector3 axis, float degrees)
        {
            GameObject = gameObject;
            Axis = axis;
            Degrees = degrees;
        }

        public void Increment(float ratio)
        {
            if (ratio == 0)
            {
                OriginalRotation = GameObject.transform.rotation;
            }
            GameObject.transform.rotation = OriginalRotation * Quaternion.AngleAxis(Degrees * ratio,
                Axis);
        }
    }
}