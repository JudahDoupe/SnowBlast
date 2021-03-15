using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DashComponent : MonoBehaviour
    {
        public readonly JBehaviorSet DashAnimation;
        private float DashEndTime;
        private Vector3 DashVector;
        private ParticleSystem DashLines = default!;

        private const float DashVelocity = 50.0f;
        private const float DashDurationSeconds = 0.2f;
        private const float DashRechargeSeconds = 0.2f;
        private Player Player = default!;

        public DashComponent()
        {
            DashAnimation = JBehaviorSet.Animate(() =>
                {
                    DashVector = Player.MoveVector.normalized;
                    Debug.Log(DashVector);
                    SetDashLines(DashVector.normalized);
                    gameObject.GetComponent<Rigidbody>()
                        .velocity = Find.CameraRotation * DashVector.normalized * DashVelocity;
                })
                .Then(DashDurationSeconds, _ =>
                {
                    gameObject.GetComponent<Rigidbody>()
                        .velocity = Find.CameraRotation * DashVector.normalized * DashVelocity;
                })
                .Then(() =>
                {
                    DashLines.Stop();
                    gameObject.GetComponent<Rigidbody>()
                        .velocity = Find.CameraRotation * DashVector.normalized * 0;
                    DashEndTime = Time.fixedTime;
                });
        }

        public void OnDash()
        {
            if (Find.PlayerState.InputBlocker.Blocked ||
                DashAnimation.InProgress ||
                Time.fixedTime < DashEndTime + DashRechargeSeconds ||
                Player.MoveVector.magnitude < 0.05)
            {
                return;
            }

            Player.StopAiming();
            StartCoroutine(DashAnimation.Start());
        }

        public void Start()
        {
            Player = GetComponent<Player>();
            DashLines = GetComponentInChildren<ParticleSystem>(true);
            var main = DashLines.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(DashDurationSeconds);
            DashLines.Stop();
        }

        private void SetDashLines(Vector3 dashVector)
        {
            var emitterTransform = DashLines.GetComponent<Transform>();
            emitterTransform.LookAt(transform.position + Find.CameraRotation * -dashVector * 10);

            DashLines.Play();
        }
    }
}