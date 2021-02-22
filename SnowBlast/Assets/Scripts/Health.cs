using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int Hitpoints;

    public Allegiance Allegiance = Allegiance.Enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Hitpoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}

public enum Allegiance
{
    Player,
    Enemy
}