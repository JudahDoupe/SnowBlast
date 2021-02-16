using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int StoppingPower;
    private bool DamageDelivered;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Lifespan());
    }

    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!DamageDelivered && collision.gameObject.TryGetComponent<Health>(out var health))
        {
            DamageDelivered = true;
            Debug.Log(collision.gameObject.name);
            health.Hitpoints -= StoppingPower;
        }
        Destroy(gameObject);
    }
}
