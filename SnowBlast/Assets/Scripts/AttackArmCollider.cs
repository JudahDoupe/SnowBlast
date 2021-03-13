using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;

public class AttackArmCollider : MonoBehaviour
{
    public bool HitDetected = false;
    void OnCollisionEnter(Collision collision)
    {
        if (!HitDetected && collision.gameObject.UltimateParent() == Find.ThePlayer)
        {
            HitDetected = true;
            Find.ThePlayer.GetComponent<Health>().ApplyDamage(50, Allegiance.Enemy);
            Debug.Log("Hit!");
        }
    }
}
