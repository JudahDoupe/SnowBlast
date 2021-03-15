using System.Collections;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts.Turret
{
    public class TurretCannon : MonoBehaviour
    {
        // ToDo: Make a barrel prefab/object that has a child muzzle?

        public GameObject Projectile;
        public Transform Muzzle1Transform;
        public Transform Muzzle2Transform;
        public float MuzzleVelocity = 50f;
        public float FireRate;
        public float MaxRange = 100f;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            var barrel = 0;
            var barrels = new []{ Muzzle1Transform, Muzzle2Transform };
            while (Find.ThePlayer != null)
            {
                yield return new WaitForSeconds(FireRate / 2);
                var muzzle = barrels[barrel];
                var bullet1 = Instantiate(Projectile, muzzle.position, muzzle.rotation);
                var b1 = bullet1.GetComponent<Bullet>();

                b1.Vector = muzzle.forward * MuzzleVelocity;
                b1.MaxRange = muzzle.forward * MaxRange;
                barrel = (barrel + 1) % 2;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
