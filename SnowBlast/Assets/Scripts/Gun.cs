using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Gun : MonoBehaviour
{
    public GameObject Projectile;
    public Transform MuzzleTransform;
    public float MaxRange = 100f;
    public float MuzzleVelocity = 10f;

    public void Attack()
    {
        var bullet = Instantiate(Projectile, MuzzleTransform.position, MuzzleTransform.rotation);
        var projectile = bullet.GetComponent<Bullet>();
        projectile.Vector = MuzzleTransform.forward * MuzzleVelocity;
        projectile.MaxRange = MuzzleTransform.forward * MaxRange;
    }
}
