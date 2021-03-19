using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public DashComponent()
        {
            DashAnimation = new JBehaviorSet().Then(() =>
                {
                    Find.PlayerState.MoveBlocker.Block(this);
                    Find.PlayerState.RotateBlocker.Block(this);
                    Find.PlayerState.AimBlocker.Block(this);
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
                    Find.PlayerState.MoveBlocker.Unblock(this);
                    Find.PlayerState.RotateBlocker.Unblock(this);
                    Find.PlayerState.AimBlocker.Unblock(this);
                });
        }

        public void OnMoveUpDown(InputValue input)
        {
            DashVector = new Vector3(DashVector.x, 0, input.Get<Vector2>().y);
        }

        public void OnMoveRightLeft(InputValue input)
        {
            DashVector = new Vector3(input.Get<Vector2>().x, 0, DashVector.z);
        }

        public void OnLeftStick(InputValue input)
        {
            DashVector = input.Get<Vector2>().ToVector3XZ();
        }

        public void OnDash()
        {
            if (DashAnimation.InProgress ||
                Time.fixedTime < DashEndTime + DashRechargeSeconds ||
                DashVector.magnitude < 0.05)
            {
                return;
            }
            
            StartCoroutine(DashAnimation.Start());
        }

        public void Start()
        {
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