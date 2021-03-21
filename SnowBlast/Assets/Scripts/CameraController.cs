using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 1.0f;

    private HashSet<GameObject> Encompassed = new HashSet<GameObject>();

    public HashSet<GameObject> GetEncompassed()
    {
        var result = new HashSet<GameObject>(Encompassed.Where(it => it != null));
        if (Find.ThePlayer is { } player) result.Add(player);
        return result;
    }

    public void Encompass(params GameObject[] gameObjects) => Encompassed.AddAll(gameObjects);

    public void Remove(params GameObject[] gameObjects) => Encompassed.RemoveAll(gameObjects);

    void Start()
    {
        var player = Find.ThePlayer;
        transform.position = TargetPosition(player.transform.position);
        transform.LookAt(player.gameObject.transform);
    }

    void FixedUpdate ()
    {
        var encompassed = GetEncompassed();

        var bounds = encompassed.GetMaxBounds();

        //var left = Camera.current.WorldToScreenPoint(bounds)

        // var summedposition = encompassed.Aggregate(Vector3.zero, (accum, current) => accum + current.transform.position)
        //                      / encompassed.Count;



        transform.position = Vector3.Lerp(transform.position, TargetPosition(bounds.center), Time.deltaTime * Speed);
    }

    private Vector3 TargetPosition(Vector3 target) => target + new Vector3(-1, 1, -1).normalized * 25;
}
