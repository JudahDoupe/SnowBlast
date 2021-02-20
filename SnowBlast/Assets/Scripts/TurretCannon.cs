using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCannon : MonoBehaviour
{
    // ToDo: Make a barrel prefab/object that has a child muzzle?

    public GameObject Projectile;
    public Transform Muzzle1Transform;
    public Transform Muzzle2Transform;
    public float ProjectileSpeed;
    public float FireRate;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate/2);
            var bullet1 = Instantiate(Projectile, Muzzle1Transform.position, Muzzle1Transform.rotation);
            bullet1.GetComponent<Rigidbody>().velocity = Muzzle1Transform.forward * ProjectileSpeed;
            yield return new WaitForSeconds(FireRate / 2);
            var bullet2 = Instantiate(Projectile, Muzzle2Transform.position, Muzzle2Transform.rotation);
            bullet2.GetComponent<Rigidbody>().velocity = Muzzle1Transform.forward * ProjectileSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
