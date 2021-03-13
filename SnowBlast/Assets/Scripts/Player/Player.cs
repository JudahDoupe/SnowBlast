using System.Collections;
using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
#nullable enable

    public class Player : MonoBehaviour
    {
        public float Speed;
        internal Vector3 MoveVector = new Vector3(0, 0, 0);
        
        private JBehaviorSet DashAnimation = default!;
        private AimingComponent AimingComponent = default!;
        private LockOnComponent LockOnComponent = default!;

        private GameObject SpeechBubble => gameObject.transform.Find("SpeechBubble").gameObject;

        public void Start()
        {
            DashAnimation = gameObject.GetComponent<DashComponent>().DashAnimation;
            AimingComponent = gameObject.GetComponent<AimingComponent>();
            LockOnComponent = GetComponent<LockOnComponent>();
        }

        public void OnMoveUpDown(InputValue input)
        {
            MoveVector = new Vector3(MoveVector.x, 0, input.Get<Vector2>().y);
        }

        public void OnMoveRightLeft(InputValue input)
        {
            MoveVector = new Vector3(input.Get<Vector2>().x, 0, MoveVector.z);
        }

        public void OnLeftStick(InputValue input)
        {
            MoveVector = input.Get<Vector2>().ToVector3XZ();
        }

        public void OnPrimaryAttack()
        {
            Debug.Log("Primary attack.");
            var gun = gameObject.GetComponentInChildren<Gun>();
            gun.Attack();
        }


        public IEnumerator Bang()
        {
            SpeechBubble.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            SpeechBubble.SetActive(false);
        }


        void FixedUpdate()
        {
            UpdateVelocity();
            UpdateFacing();
        }

        private void UpdateFacing()
        {
            if (DashAnimation.InProgress) return;
            if (LockOnComponent.LockOnTarget is {} target)
            {
                var xyz = target.transform.position;
                gameObject.transform.LookAt(new Vector3(xyz.x, 0, xyz.z));
            }
            else if (Gamepad.current == null)
            {
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out var hit))
                {
                    var x = hit.point;
                    x.y = transform.position.y;
                    gameObject.transform.LookAt(x);
                }
            }
            else
            {
                var lookPosition = gameObject.transform.position + Find.CameraRotation * MoveVector;
                gameObject.transform.LookAt(lookPosition);
            }
        }

        private void UpdateVelocity()
        {
            if (DashAnimation.InProgress) return;
            if (AimingComponent.Aiming)
            {
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else if (MoveVector.magnitude > 0)
            {
                gameObject.GetComponent<Rigidbody>()
                    .velocity = Find.CameraRotation * MoveVector * Speed;
            }
        }

        
        public void StopAiming() => GetComponent<AimingComponent>().StopAiming();
    }
}