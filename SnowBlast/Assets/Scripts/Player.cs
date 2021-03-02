using System;
using System.Linq;
using Assets.Scripts;
using Assets.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float Speed;
    private Vector2 moveVec = new Vector2(0, 0);
    private int LastAttackFrame = -1;
    private GameObject LockOnTarget = null;
    private IDisposable LockOnDispose = null;

    // Start is called before the first frame update
    void Start()
    {
    }

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
            var lookPosition = gameObject.transform.position + new Vector3(moveVec.x, 0, moveVec.y) * Speed;
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

    public void OnSwitchLockOn(InputValue input)
    {
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;

        var direction = input.Get<Vector2>();

        if (direction.magnitude < 0.5f) return;

        if (LockOnTarget == null) return;

        // Find the enemy that's closest relative to current selection and angle of stick
        var arenaController = GameObject.Find("ArenaController").GetComponent<ArenaController>();
        var otherEnemies = arenaController.Enemies.Where(enemy => enemy != LockOnTarget).ToList();
        if (otherEnemies.Count == 0) return;

        direction = direction.normalized;
        var startingPoint = LockOnTarget.transform.position;

        var ray = new Ray(startingPoint, direction);

        LockOnTarget = otherEnemies.MinBy(enemy => Vector3.Cross(ray.direction, enemy.transform.position - ray.origin).magnitude);
    }

    public void OnToggleLockOn()
    {
        if (LastAttackFrame == Time.frameCount) return;
        LastAttackFrame = Time.frameCount;

        if (LockOnTarget == null)
        {
            // Find the closest enemy and lock onto them.
            var arenaController = GameObject.Find("ArenaController").GetComponent<ArenaController>();

            LockOnTarget = arenaController.Enemies.MinBy(enemy => Vector3.Distance(enemy.transform.position, gameObject.transform.position));

            var indicator = LockOnTarget.GetComponentInChildren<EnemyInfoDisplay>();
            indicator.StartLockOn();

            var enemyHealth = LockOnTarget.GetComponentInChildren<Health>();

            LockOnDispose = enemyHealth.Subscribe(hn =>
            {
                if (hn.CurrentHealth <= 0)
                {
                    LockOnTarget = null;
                    LockOnDispose = null;
                }
            });
        }
        else
        {
            var indicator = LockOnTarget.GetComponentInChildren<EnemyInfoDisplay>();
            indicator.StopLockOn();
            LockOnTarget = null;
            LockOnDispose.Dispose();
            LockOnDispose = null;
        }
    }

    void FixedUpdate()
    {
        if (moveVec.x != 0 || moveVec.y != 0)
        {
            gameObject.GetComponent<Rigidbody>()
                .velocity = Quaternion.AngleAxis(45, Vector3.up) * new Vector3(moveVec.x, 0, moveVec.y) * Speed;
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
}
