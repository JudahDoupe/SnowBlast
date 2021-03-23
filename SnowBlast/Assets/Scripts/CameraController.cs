using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 1.0f;

    private readonly HashSet<GameObject> Encompassed = new HashSet<GameObject>();

    private float MinOrthographicSize = 5.0f;

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

        var camera = GetComponent<Camera>();
        MinOrthographicSize = camera.orthographicSize;
    }

    void FixedUpdate ()
    {
        var encompassed = GetEncompassed();
        var targetOrthographicSize = MinOrthographicSize;
        if (encompassed.Count == 0)
        {
            return;
        }

        Vector3 target;
        var camera = GetComponent<Camera>();
        if (encompassed.Count == 1)
        {
            target = encompassed.First().transform.position;
        }
        else
        {
            var verticalSizeHeuristic = 1.0f;
            var horizontalSizeHeuristic =
                verticalSizeHeuristic * Screen.currentResolution.width / Screen.currentResolution.height;
            

            var bounds = encompassed.GetMaxBounds();
            target = bounds.center;

            var points = bounds.Corners()
                .Select(corner => Quaternion.LookRotation(transform.forward, transform.up) * corner)
                .ToList();

            var minX = points.Select(it => it.x).Min();
            var maxX = points.Select(it => it.x).Max();
            var minY = points.Select(it => it.y).Min();
            var maxY = points.Select(it => it.y).Max();

            var deltaX = (maxX - minX) / horizontalSizeHeuristic;
            var deltaY = (maxY - minY) / verticalSizeHeuristic;
            targetOrthographicSize = Mathf.Max(Mathf.Max(deltaY, deltaX) / 2, MinOrthographicSize);
        }

        
        if (Math.Abs(camera.orthographicSize - targetOrthographicSize) > 0.05f)
        {
            var newcs = Mathf.Lerp(camera.orthographicSize, targetOrthographicSize, Time.deltaTime * Speed);
            camera.orthographicSize = newcs;
        }

        transform.position = Vector3.Lerp(transform.position, TargetPosition(target), Time.deltaTime * Speed);
    }

    private Vector3 TargetPosition(Vector3 target) => target + new Vector3(-1, 1, -1).normalized * 25;
}
