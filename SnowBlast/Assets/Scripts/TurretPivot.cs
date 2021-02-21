using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretPivot : MonoBehaviour
{
    public float PivotSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            var player = players.First();
            var lookQuat = Quaternion.LookRotation(player.transform.position - gameObject.transform.position, Vector3.up);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, lookQuat, PivotSpeed * Time.deltaTime);
        }
    }
}