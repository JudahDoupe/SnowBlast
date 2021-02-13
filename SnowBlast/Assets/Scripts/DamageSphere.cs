using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DoDamage());
        }
    }

    private IEnumerator DoDamage()
    {
        gameObject.transform.localScale = new Vector3(4, 4, 4);

        var colliders = Physics.OverlapSphere(gameObject.transform.position, 4);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<Health>(out var health))
            {
                health.Hitpoints -= 50;
            }
        }

        yield return new WaitForSeconds(0.5f);
        gameObject.transform.localScale = new Vector3(0,0,0);
    }
}
