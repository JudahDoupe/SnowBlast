using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject Projectile;
    public Transform MuzzleTransform;
    public int Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Attack()
    {
        var bullet = Instantiate(Projectile, MuzzleTransform.position, MuzzleTransform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = MuzzleTransform.forward * Speed;
    }
}
