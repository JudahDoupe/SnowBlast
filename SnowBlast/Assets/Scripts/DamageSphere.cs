using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

public class DamageSphere : MonoBehaviour
{
    public float Radius = 4;
    public float Speed = 5;
    public int Damage = 50;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
    }

    public void Attack()
    {
         StartCoroutine(DoDamage());
    }

    private IEnumerator DoDamage()
    {
        var targetScale = new Vector3(1, 1, 1) * Radius * 2;

        while (Vector3.Distance(gameObject.transform.localScale, targetScale) > 0.01f)
        {
            gameObject.transform.localScale =
                Vector3.Lerp(gameObject.transform.localScale, targetScale, Time.deltaTime * Speed);
            yield return new WaitForEndOfFrame();
        }

        var colliders = Physics.OverlapSphere(gameObject.transform.position, Radius);
        var hit = new HashSet<int>();
        foreach (var collisionObject in colliders.Select(collider => collider.gameObject.UltimateParent()))
        {
            if (!hit.Contains(collisionObject.GetInstanceID()) && 
                collisionObject.TryGetComponent<Health>(out var health))
            {
                hit.Add(collisionObject.GetInstanceID());
                health.Hitpoints -= Damage;
            }
        }
        gameObject.transform.localScale = new Vector3(0,0,0);
    }
}
