using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public float Speed;
    public float ZoomSpeed;

    public float MinDistance;
    public float MaxDistance;
    private float distance;

    public float MinAngle;
    public float MaxAngle;
    private float angle;

    void Start()
    {
        distance = (MaxDistance - MinDistance) / 2 + MinDistance;
    }

    void FixedUpdate ()
    {
        //distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y * ZoomSpeed, MinDistance, MaxDistance);
        //angle = Mathf.Lerp(MinAngle, MaxAngle, (distance - MinDistance) / (MaxDistance - MinDistance));

        //var targetPosition = Player.transform.position + new Vector3(-1,1,-1) * distance;
        //transform.LookAt(Player.gameObject.transform);
        //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * Speed);
        //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(angle, 0, 0), Time.deltaTime * ZoomSpeed);
    }
}
