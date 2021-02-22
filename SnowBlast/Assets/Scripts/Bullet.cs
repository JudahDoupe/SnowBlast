using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int StoppingPower;
    private bool DamageDelivered;
    public Allegiance Allegiance = Allegiance.Enemy;

    [HideInInspector]
    public Vector3 Vector;
    [HideInInspector]
    public Vector3 MaxRange;

    private Vector3 InstantiatedPosition;

    void Start()
    {
        InstantiatedPosition = gameObject.transform.position;
        GetComponent<Rigidbody>().velocity = Vector;
    }

    void Update()
    {
        var currentPosition = gameObject.transform.position;
        if (Vector3.Distance(InstantiatedPosition, currentPosition) >
            Vector3.Distance(InstantiatedPosition, MaxRange))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!DamageDelivered && collision.gameObject.TryGetComponent<Health>(out var health))
        {
            if (health.Allegiance != Allegiance)
            {
                DamageDelivered = true;
                health.Hitpoints -= StoppingPower;
            }
        }
        Destroy(gameObject);
    }
}
