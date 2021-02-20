using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed;

    private GameObject Player;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        transform.position = TargetPosition;
        transform.LookAt(Player.gameObject.transform);
    }

    void FixedUpdate ()
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition, Time.deltaTime * Speed);
    }

    private Vector3 TargetPosition => Player.transform.position + new Vector3(-1, 1, -1).normalized * 25;
}
