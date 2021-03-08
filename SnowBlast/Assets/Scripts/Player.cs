using System;
using System.Collections;
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
    private Vector3 MoveVector = new Vector3(0, 0, 0);
    private int LastAttackFrame = -1;
    private GameObject LockOnTarget = null;
    private IDisposable LockOnDispose = null;
    private bool RightStickBumped = false;
    private const float BumpTrigger = 0.5f;
    private const float BumpReset = 0.1f;

    private const float DashVelocity = 50.0f;
    private const float DashDurationSeconds = 0.2f;
    private const float DashRechargeSeconds = 0.2f;

    private float? DashStarted;
    private float DashEnded = 0.0f;
    private Vector3 DashVector;
    private AimingLines AimingLines;
    private bool Aiming => AimingLines.gameObject.activeSelf;
    private GameObject SpeechBubble => gameObject.transform.Find("SpeechBubble").gameObject;


    public void Start()
    {
        var emitter = GetComponentInChildren<ParticleSystem>(true);
        var main = emitter.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(DashDurationSeconds);
        SetEmitter(false);
        AimingLines = FindObjectOfType<LineRenderer>(true).GetComponent<AimingLines>();
    }

    private void SetEmitter(bool active)
    {
        var emitter = GetComponentInChildren<ParticleSystem>(true);
        if(active) emitter.Play();
        else emitter.Stop();

        if (active)
        {
            var emitterTransform = emitter.GetComponent<Transform>();

            var angle = (LockOnTarget, MoveVector.x < 0) switch
            {
                (null, _) => 180,
                (_, true) => 90,
                _ => -90
            };

            emitterTransform.rotation = transform.rotation * Quaternion.AngleAxis(angle, new Vector3(0, 1, 0));
        }
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
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;
        var gun = gameObject.GetComponentInChildren<Gun>();
        gun.Attack();
    }

    public void OnSecondaryAttack(InputValue action)
    {
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;

        if (action.isPressed && DashStarted == null)
        {
            AimingLines.transform.position = transform.position;
            AimingLines.transform.rotation = transform.rotation;
            AimingLines.StartAnimation(() =>
            {
                var range = AimingLines.Range;
                var origin = new Vector3(transform.position.x, 1.01f, transform.position.z);
                if (Physics.Raycast(origin, transform.TransformDirection(Vector3.forward), out var hit))
                {
                    if (hit.distance <= range && hit.collider.gameObject.UltimateParent().GetComponent<Health>() is {} health)
                    {
                        StartCoroutine(Bang());
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

    IEnumerator Bang()
    {
        SpeechBubble.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        SpeechBubble.SetActive(false);
    }

    public void OnRightStick(InputValue input)
    {
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

        var arenaController = GameObject.Find("ArenaController").GetComponent<ArenaController>();
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

    public void OnDash()
    {
        StopAiming();
        if (DashStarted != null) return;
        if (Time.fixedTime < DashEnded + DashRechargeSeconds) return;
        if (MoveVector.magnitude < 0.05) return;

        SetEmitter(true);

        DashStarted = Time.fixedTime;
        DashVector = MoveVector.normalized;
    }

    private void StopAiming()
    {
        AimingLines.StopAnimation();
    }

    void FixedUpdate()
    {
        if (DashStarted != null)
        {
            if (Time.fixedTime >= DashStarted + DashDurationSeconds)
            {
                DashEnded = Time.fixedTime;
                SetEmitter(false);
                DashStarted = null;
            }
            else
            {
                gameObject.GetComponent<Rigidbody>()
                    .velocity = Find.CameraRotation * DashVector * DashVelocity;
            }
        }
        else if (Aiming)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;;
        }
        else if (MoveVector.magnitude > 0)
        {
            gameObject.GetComponent<Rigidbody>()
                .velocity = Find.CameraRotation * MoveVector * Speed;
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
        else
        {
            var lookPosition = gameObject.transform.position + Find.CameraRotation * MoveVector;
            gameObject.transform.LookAt(lookPosition);
        }

        if (Aiming)
        {
            AimingLines.transform.rotation = transform.rotation;
        }
    }

    public enum Facing
    {
        Front,
        Side,
        Rear
    }
}
