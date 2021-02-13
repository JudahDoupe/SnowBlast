using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public float Speed;
    public float Distance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var targetPosition = Player.transform.position + new Vector3(0,1,-1) * Distance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * Speed);
    }
}
