using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    public class AimingComponent : MonoBehaviour
    {
        private AimingLines AimingLines = default!;
        public bool Aiming => AimingLines?.Animation.InProgress == true;
        public JBehaviorSet DashAnimation;
        private Player Player;

        void Start()
        {
            AimingLines = GetComponentInChildren<AimingLines>(true);
            DashAnimation = GetComponent<DashComponent>().DashAnimation;
            Player = GetComponent<Player>();
        }

        public void StopAiming()
        {
            AimingLines.StopAnimation();
        }

        public void OnSecondaryAttack(InputValue action)
        {
            if (DashAnimation.InProgress) return;

            if (action.isPressed)
            {
                AimingLines.transform.position = transform.position;
                AimingLines.transform.rotation = transform.rotation;
                AimingLines.StartAnimation(() =>
                {
                    var range = AimingLines.Range;
                    var origin = new Vector3(transform.position.x, 1.01f, transform.position.z);
                    if (Physics.Raycast(origin, transform.TransformDirection(Vector3.forward), out var hit))
                    {
                        if (hit.distance <= range && hit.collider.gameObject.UltimateParent().GetComponent<Health>() is { } health)
                        {
                            StartCoroutine(Player.Bang());
                            health.ApplyDamage(300, Allegiance.Player);
                        }
                    }
                });
            }
            else
            {
                AimingLines.StopAnimation();
            }
        }
    }
}