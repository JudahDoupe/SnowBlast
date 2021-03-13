using System;
using System.Collections;
using System.Linq;
using Assets.Scripts;
using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable
public class Player : MonoBehaviour
{
    public float Speed;
    private Vector3 MoveVector = new Vector3(0, 0, 0);
    private int LastAttackFrame = -1;
    private GameObject? LockOnTarget = null;
    private IDisposable? LockOnDispose = null;
    private bool RightStickBumped = false;
    private const float BumpTrigger = 0.5f;
    private const float BumpReset = 0.1f;

    private const float DashVelocity = 50.0f;
    private const float DashDurationSeconds = 0.2f;
    private const float DashRechargeSeconds = 0.2f;

    private AimingLines AimingLines = default!;
    private ParticleSystem DashLines = default!;
    private bool Aiming => AimingLines.gameObject.activeSelf;
    private GameObject SpeechBubble => gameObject.transform.Find("SpeechBubble").gameObject;

    private readonly JBehaviorSet DashAnimation;
    private float DashEndTime;

    public Player()
    {
        DashAnimation = JBehaviorSet.Animate(() =>
            {
                SetDashLines(MoveVector.normalized);
                gameObject.GetComponent<Rigidbody>()
                    .velocity = Find.CameraRotation * MoveVector.normalized * DashVelocity;
            })
            .Then(DashDurationSeconds, _ =>
            {
                Debug.Log("Hello world!");
                gameObject.GetComponent<Rigidbody>()
                    .velocity = Find.CameraRotation * MoveVector.normalized * DashVelocity;
            })
            .Then(() =>
            {
                DashLines.Stop();
                gameObject.GetComponent<Rigidbody>()
                    .velocity = Find.CameraRotation * MoveVector.normalized * 0;
                DashEndTime = Time.fixedTime;
            });
    }

    public void Start()
    {
        AimingLines = gameObject.GetComponentInChildren<AimingLines>(true);
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
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;
        var gun = gameObject.GetComponentInChildren<Gun>();
        gun.Attack();
    }

    public void OnSecondaryAttack(InputValue action)
    {
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;

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

    private void StopLockOn()
    {
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
        if (DashAnimation.InProgress) return;
        if (Time.fixedTime < DashEndTime + DashRechargeSeconds) return;
        if (MoveVector.magnitude < 0.05) return;
        StopAiming();
        StartCoroutine(DashAnimation.Start());
    }

    private void StopAiming()
    {
        AimingLines.StopAnimation();
    }

    void FixedUpdate()
    {
        UpdateVelocity();
        UpdateFacing();

        if (Aiming)
        {
            AimingLines.transform.rotation = transform.rotation;
        }
    }

    private void UpdateFacing()
    {
        if (DashAnimation.InProgress) return;
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
    }

    private void UpdateVelocity()
    {
        if (DashAnimation.InProgress) return;
        if (Aiming)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if (MoveVector.magnitude > 0)
        {
            gameObject.GetComponent<Rigidbody>()
                .velocity = Find.CameraRotation * MoveVector * Speed;
        }
    }

    public enum Facing
    {
        Front,
        Side,
        Rear
    }
}
