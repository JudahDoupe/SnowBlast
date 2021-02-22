using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed;

    void Start()
    {
        var player = GameObject.FindWithTag("Player");
        transform.position = TargetPosition(player);
        transform.LookAt(player.gameObject.transform);
    }

    void FixedUpdate ()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            transform.position = Vector3.Lerp(transform.position, TargetPosition(player), Time.deltaTime * Speed);
        }
    }

    private Vector3 TargetPosition(GameObject player) => player.transform.position + new Vector3(-1, 1, -1).normalized * 25;
}
