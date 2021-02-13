using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var vector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        gameObject.GetComponent<Rigidbody>()
            .velocity = new Vector3(vector.x, 0, vector.y) * Speed;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            var x = hit.point;
            x.y = transform.position.y;
            gameObject.transform.LookAt(x);
        }
    }
}
