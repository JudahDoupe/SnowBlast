using System;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class AimingLines : MonoBehaviour
    {
        public float AnimationDuration = 0.5f;
        public float StartingAngle = 90;
        public float Range = 12.0f;

        private LineRenderer LineRenderer => GetComponent<LineRenderer>();
    
        public readonly StartableTweener Animation;

        public AimingLines()
        {
            Animation = this.BeginSerial().Then(() => gameObject.SetActive(true))
                .Then(AnimationDuration, ratio => RenderAngle(StartingAngle * (1.0f - ratio)))
                .Then(() => gameObject.SetActive(false));
        }

        // Start is called before the first frame update
        void Start()
        {
            LineRenderer.startWidth = 0.2f;
            LineRenderer.endWidth = 0.2f;
        }

        public void StartAnimation(Action onComplete)
        {
            Animation.Start(onComplete);
        }

        public void StopAnimation()
        {
            gameObject.SetActive(false);
            Animation.Stop();
        }

        private void RenderAngle(float newAngle)
        {
            var halfAngle = newAngle / 2;
            var height = 1.0f;
            LineRenderer.SetPositions(new[]
            {
                new Vector3(Range * MathInDegrees.Sin(halfAngle), height, Range * MathInDegrees.Cos(halfAngle)),
                new Vector3(0, height, 0),
                new Vector3(-Range * MathInDegrees.Sin(halfAngle), height, Range * MathInDegrees.Cos(halfAngle)),
            });
        }
    }
}
