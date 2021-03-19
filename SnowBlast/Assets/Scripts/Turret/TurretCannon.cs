using System.Collections;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts.Turret
{
    public class TurretCannon : MonoBehaviour
    {
        // ToDo: Make a barrel prefab/object that has a child muzzle?

        public GameObject Projectile;
        private Transform Muzzle1Transform;
        private Transform Muzzle2Transform;
        public float MuzzleVelocity = 15f;
        public float FireDelay = 0.5f;
        public float MaxRange = 20f;

        // Start is called before the first frame update
        void Start()
        {
            Muzzle1Transform = transform.Find("Barrel1").Find("Muzzle1");
            Muzzle2Transform = transform.Find("Barrel2").Find("Muzzle2");
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            var barrel = 0;
            var barrels = new []{ Muzzle1Transform, Muzzle2Transform };
            while (Find.ThePlayer != null)
            {
                yield return new WaitForSeconds(FireDelay);
                var muzzle = barrels[barrel];
                var bullet1 = Instantiate(Projectile, muzzle.position, muzzle.rotation);
                var b1 = bullet1.GetComponent<Bullet>();

                b1.Vector = muzzle.forward * MuzzleVelocity;
                b1.MaxRange = muzzle.forward * MaxRange;
                barrel = (barrel + 1) % 2;
            }
        }
    }
}
