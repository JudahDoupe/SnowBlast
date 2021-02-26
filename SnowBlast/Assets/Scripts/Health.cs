using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int Hitpoints;

    [HideInInspector] 
    public bool Invulnerable;

    public Allegiance Allegiance = Allegiance.Enemy;

    [HideInInspector]
    public int MaxHealth { get; private set; }

    [HideInInspector]
    public int CurrentHealth => Hitpoints;

    public void Start()
    {
        MaxHealth = Hitpoints;
    }

    public void ApplyDamage(int amount, Allegiance damageSource)
    {
        if (Invulnerable) return;
        if (damageSource == Allegiance) return;
        Hitpoints -= amount;
        if (Hitpoints <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

public enum Allegiance
{
    Player,
    Enemy
}