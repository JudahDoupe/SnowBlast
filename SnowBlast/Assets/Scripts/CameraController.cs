using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed = 1.0f;

    private HashSet<GameObject> Encompassed = new HashSet<GameObject>();

    private float Pullback = 5.0f;
    private float MinPullback = 5.0f;
    private float PullbackStep = 0.2f;

    public HashSet<GameObject> GetEncompassed()
    {
        var result = new HashSet<GameObject>(Encompassed.Where(it => it != null));
        if (Find.ThePlayer is { } player) result.Add(player);
        return result;
    }

    public void Encompass(params GameObject[] gameObjects) => Encompassed.AddAll(gameObjects);

    public void Remove(params GameObject[] gameObjects) => Encompassed.RemoveAll(gameObjects);

    private Bounds BoundsLastUpdate;
    private bool PullingBack;

    void Start()
    {
        var player = Find.ThePlayer;
        transform.position = TargetPosition(player.transform.position);
        transform.LookAt(player.gameObject.transform);
    }

    void FixedUpdate ()
    {
        var encompassed = GetEncompassed();

        if (encompassed.Count == 1)
        {
            PullingBack = false;
            Pullback = Mathf.Max(Pullback - PullbackStep, MinPullback);
            Camera.main.orthographicSize = Pullback;
            transform.position = Vector3.Lerp(transform.position, TargetPosition(encompassed.First().transform.position), 
                Time.deltaTime * Speed);
            return;
        }

        var bounds = encompassed.GetMaxBounds();

        if (bounds != BoundsLastUpdate || PullingBack)
        {
            BoundsLastUpdate = bounds;

            if (bounds.Corners().Any(corner =>
            {
                var sp = Camera.main.WorldToScreenPoint(corner);
                return sp.x < 0 || sp.y < 0 ||
                       sp.x > Screen.currentResolution.width
                       || sp.y > Screen.currentResolution.height;
            }))
            {
                PullingBack = true;
                Pullback += PullbackStep;
            }
            else if (!PullingBack)
            {
                Pullback = Mathf.Max(Pullback - PullbackStep, MinPullback);
            }
            else
            {
                PullingBack = false;
            }

            Camera.main.orthographicSize = Pullback;
        }

        transform.position = Vector3.Lerp(transform.position, TargetPosition(bounds.center), Time.deltaTime * Speed);
    }

    private Vector3 TargetPosition(Vector3 target) => target + new Vector3(-1, 1, -1).normalized * 25;
}
