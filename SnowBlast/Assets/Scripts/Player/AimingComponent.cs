using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
#nullable enable
    public class AimingComponent : MonoBehaviour
    {
        private AimingLines? AimingLines;

        void Start()
        {
            AimingLines = GetComponentInChildren<AimingLines>(true);
            Find.PlayerState.AimBlocker.Subscribe(state =>
            {
                if (state == BlockerStack.BlockState.Blocked)
                {
                    StopAiming();
                }
            });
        }

        public void StopAiming()
        {
            AimingLines?.StopAnimation();
        }

        public void OnSecondaryAttack(InputValue action)
        {
            if (Find.PlayerState.WeaponsFree) return;
            if (Find.PlayerState.AimBlocker.IsBlocked) return;
            if (AimingLines == null) return;
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
                            Bang();
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

        private void Bang()
        {
            var unsay = Sayer.Say(Find.ThePlayer, "Bang!");
            StartCoroutine(new JBehaviorSet()
                .Wait(2.0f)
                .Start(() => unsay()));
        }
    }
}