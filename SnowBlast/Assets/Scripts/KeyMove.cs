using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMove : MonoBehaviour
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
        if (vector.magnitude > 0.1)
        {
            vector.Normalize();
            gameObject.GetComponent<Rigidbody>()
                .velocity = new Vector3(vector.x, 0, vector.y) * -Speed;
        }
    }
}
