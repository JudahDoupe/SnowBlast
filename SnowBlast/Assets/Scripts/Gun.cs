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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var bullet = Instantiate(Projectile, MuzzleTransform.position, MuzzleTransform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = MuzzleTransform.forward * Speed;
        }  
    }
}
