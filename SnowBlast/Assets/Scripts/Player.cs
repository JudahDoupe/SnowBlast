using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float Speed;
    private Vector2 moveVec = new Vector2(0, 0);
    private int LastAttackFrame = -1;

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
        var lookPosition = gameObject.transform.position + new Vector3(moveVec.x, 0, moveVec.y) * Speed;
        gameObject.transform.LookAt(lookPosition);
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

    void FixedUpdate()
    {
        if (moveVec.x != 0 || moveVec.y != 0)
        {
            gameObject.GetComponent<Rigidbody>()
                .velocity = Quaternion.AngleAxis(45, Vector3.up) * new Vector3(moveVec.x, 0, moveVec.y) * Speed;
        }

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
    }
}
