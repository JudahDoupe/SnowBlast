using Assets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
#nullable enable

    public class Player : MonoBehaviour
    {
        public float Speed = 5.0f;
        internal Vector3 MoveVector = new Vector3(0, 0, 0);

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
            if (Find.PlayerState.WeaponsBlocked.IsTrue) return;
            var gun = gameObject.GetComponentInChildren<Gun>();
            gun.Attack();
        }


        void FixedUpdate()
        {
            UpdateVelocity();
            UpdateFacing();
        }

        private void UpdateFacing()
        {
            if (Find.PlayerState.RotateBlocked.IsTrue) return;
            if (Gamepad.current == null)
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
            if (Find.PlayerState.MoveBlocked.IsTrue) return;
            if (MoveVector.magnitude > 0)
            {
                gameObject.GetComponent<Rigidbody>()
                    .velocity = Find.CameraRotation * MoveVector * Speed;
            }
        }
    }
}