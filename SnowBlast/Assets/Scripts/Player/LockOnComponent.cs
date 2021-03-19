using System;
using System.Linq;
using Assets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
#nullable enable
    public class LockOnComponent : MonoBehaviour
    {
        public GameObject? LockOnTarget { get; private set; } = null;
        private IDisposable? LockOnDispose = null;
        private bool RightStickBumped = false;
        private const float BumpTrigger = 0.5f;
        private const float BumpReset = 0.1f;

        public void OnRightStick(InputValue input)
        {
            if (Find.PlayerState.WeaponsBlocker.IsBlocked) return;

            var direction = (Find.CameraRotation * input.Get<Vector2>().ToVector3XZ()).ToVector2XZ();

            if (RightStickBumped)
            {
                if (direction.magnitude < BumpReset)
                {
                    RightStickBumped = false;
                }

                return;
            }

            if (direction.magnitude <= BumpTrigger) return;

            RightStickBumped = true;

            var ac = GameObject.Find("ArenaController");
            if (ac == null) return;

            var arenaController = ac.GetComponent<ArenaController>();
            if (!arenaController) return;
            var originObject = LockOnTarget ?? Find.ThePlayer ?? throw new ApplicationException("No locked on enemy or player!");
            var initialPosition = originObject.transform.position.ToVector2XZ();
            var otherEnemies = arenaController.Enemies
                .Where(enemy => enemy != LockOnTarget)
                .Select(enemy =>
                {
                    var angle = Vector2.Angle(direction,
                        enemy.transform.position.ToVector2XZ() - initialPosition);
                    var facing = angle switch
                    {
                        _ when angle < 45 => Facing.Front,
                        _ when angle < 135 => Facing.Side,
                        _ => Facing.Rear
                    };
                    return new
                    {
                        enemy,
                        facing
                    };
                })
                .ToList();

            if (otherEnemies.Count == 0) return;

            var partitions = otherEnemies
                .GroupBy(it => it.facing, it => it.enemy)
                .ToDictionary(it => it.Key, it => it.ToList());

            var lookAtEnemies = partitions.GetValueOrDefault(Facing.Front) ??
                                partitions.GetValueOrDefault(Facing.Side) ??
                                partitions[Facing.Rear];

            var newTarget = lookAtEnemies.MinBy(enemy => Vector3.Distance(enemy.transform.position, originObject.transform.position));

            SetLockOnTarget(newTarget);
        }

        void FixedUpdate()
        {
            if (LockOnTarget is {} target)
            {
                var xyz = target.transform.position;
                transform.LookAt(new Vector3(xyz.x, 0, xyz.z));
            }
        }

        private void StopLockOn()
        {
            Find.PlayerState.RotateBlocker.Unblock(this);
            LockOnDispose?.Dispose();
            LockOnDispose = null;
            LockOnTarget = null;
        }

        private void SetLockOnTarget(GameObject? newTarget)
        {
            if (LockOnTarget == newTarget) return;

            StopLockOn();

            if (newTarget == null) return;
            LockOnTarget = newTarget;

            Find.PlayerState.RotateBlocker.Block(this);

            var indicator = LockOnTarget.GetComponentInChildren<EnemyInfoDisplay>();
            indicator.StartLockOn();

            var enemyHealth = LockOnTarget.GetComponentInChildren<Health>();

            var tempDispose = enemyHealth.Subscribe(hn =>
            {
                if (hn.CurrentHealth <= 0)
                {
                    SetLockOnTarget(null);
                }
            });

            LockOnDispose = new ActionDisposer(() =>
            {
                tempDispose.Dispose();
                indicator.StopLockOn();
            });
        }

        public void OnCancelLockOn()
        {
            StopLockOn();
        }
    }

    public enum Facing
    {
        Front,
        Side,
        Rear
    }

}