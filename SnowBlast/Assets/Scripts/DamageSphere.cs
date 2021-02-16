using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSphere : MonoBehaviour
{
    public float Radius = 4;
    public float Speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick 1 button 2"))
        {
            StartCoroutine(DoDamage());
        }
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
        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<Health>(out var health))
            {
                health.Hitpoints -= 50;
            }
        }
        gameObject.transform.localScale = new Vector3(0,0,0);
    }
}
