using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts.Shoggoth
{
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
}
