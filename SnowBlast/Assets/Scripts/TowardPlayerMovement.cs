using Assets.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowardPlayerMovement : MonoBehaviour
{
    public float Speed;
    public float MinDistanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var player = Find.ThePlayer;
        if (player != null)
        {
            var currPos = gameObject.transform.position;
            var playerPos = player.transform.position;
            if (Vector3.Distance(currPos, playerPos) > MinDistanceToPlayer)
            {
                gameObject.transform.position = Vector3.MoveTowards(currPos, playerPos, Speed * Time.deltaTime);
            }
        }
    }
}
