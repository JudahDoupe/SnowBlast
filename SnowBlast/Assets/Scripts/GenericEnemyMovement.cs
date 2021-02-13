using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemyMovement : MonoBehaviour
{
    public float Speed;
    public float MovementDelay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        var body = gameObject.GetComponent<Rigidbody>();

        while (true)
        {
            yield return new WaitForSeconds(MovementDelay);
            body.velocity = new Vector3(1, 0, 0) * Speed;
            yield return new WaitForSeconds(MovementDelay);
            body.velocity = new Vector3(0, 0, 1) * Speed;
            yield return new WaitForSeconds(MovementDelay);
            body.velocity = new Vector3(-1, 0, 0) * Speed;
            yield return new WaitForSeconds(MovementDelay);
            body.velocity = new Vector3(0, 0, -1) * Speed;
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
