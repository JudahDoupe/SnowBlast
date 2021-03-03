using System;
using System.Linq;
using Assets.Scripts;
using Assets.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public float Speed;
    private Vector2 moveVec = new Vector2(0, 0);
    private int LastAttackFrame = -1;
    private GameObject LockOnTarget = null;
    private IDisposable LockOnDispose = null;
    private bool RightStickBumped = false;
    private const float BumpTrigger = 0.5f;
    private const float BumpReset = 0.1f;

    public void OnMoveUpDown(InputValue input)
    {
        moveVec = new Vector2(moveVec.x, input.Get<Vector2>().y);
    }

    public void OnMoveRightLeft(InputValue input)
    {
        moveVec = new Vector2(input.Get<Vector2>().x, moveVec.y);
    }

    public void OnLeftStick(InputValue input)
    {
        moveVec = input.Get<Vector2>();
        if (LockOnTarget == null)
        {
            var lookPosition = gameObject.transform.position + Find.CameraRotation * moveVec.XZ();
            gameObject.transform.LookAt(lookPosition);
        }
    }

    public void OnPrimaryAttack()
    {
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;
        var gun = gameObject.GetComponentInChildren<Gun>();
        gun.Attack();
    }

    public void OnSecondaryAttack()
    {
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;
        var damageSphere = gameObject.GetComponentInChildren<DamageSphere>();
        damageSphere.Attack();
    }

    public void OnRightStick(InputValue input)
    {
        var direction = (Find.CameraRotation * input.Get<Vector2>().XZ()).XZ();

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

        var arenaController = GameObject.Find("ArenaController").GetComponent<ArenaController>();
        var originObject = LockOnTarget ?? Find.ThePlayer ?? throw new ApplicationException("No locked on enemy or player!");
        var initialPosition = originObject.transform.position.XZ();
        var otherEnemies = arenaController.Enemies
            .Where(enemy => enemy != LockOnTarget)
            .Select(enemy =>
            {
                var angle = Vector2.Angle(direction,
                    enemy.transform.position.XZ() - initialPosition);
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

    private void StopLockOn()
    {
        LockOnDispose?.Dispose();
        LockOnDispose = null;
        LockOnTarget = null;
    }

    private void SetLockOnTarget(GameObject newTarget)
    {
        if (LockOnTarget == newTarget) return;

        StopLockOn();

        if (newTarget == null) return;
        LockOnTarget = newTarget;

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

    void FixedUpdate()
    {
        if (moveVec.x != 0 || moveVec.y != 0)
        {
            gameObject.GetComponent<Rigidbody>()
                .velocity = Find.CameraRotation * new Vector3(moveVec.x, 0, moveVec.y) * Speed;
        }

        if (LockOnTarget != null)
        {
            var xyz = LockOnTarget.transform.position;
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
    }

    public enum Facing
    {
        Front,
        Side,
        Rear
    }
}
