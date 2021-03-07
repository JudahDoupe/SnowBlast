using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;

public class AimingLines : MonoBehaviour
{
    public float AnimationDuration = 0.5f;
    public float StartingAngle = 90;
    public float Range = 12.0f;
    

    private LineRenderer LineRenderer => GetComponent<LineRenderer>();
    private readonly Notifier<bool> NotifierImplementation = new Notifier<bool>();
    
    private float? AnimationStartTime = null;

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer.startWidth = 0.2f;
        LineRenderer.endWidth = 0.2f;
    }

    public void StartAnimation(Action action)
    {
        NotifierImplementation.Subscribe(_ => action());
        gameObject.SetActive(true);
        AnimationStartTime = Time.fixedTime;
        RenderAngle(StartingAngle);
    }

    public void StopAnimation()
    {
        gameObject.SetActive(false);
        AnimationStartTime = null;
        NotifierImplementation.Clear();
    }

    private void RenderAngle(float newAngle)
    {
        var halfAngle = newAngle / 2;
        var height = 1.0f;
        LineRenderer.SetPositions(new Vector3[]
        {
            new Vector3(Range * MathInDegrees.Sin(halfAngle), height, Range * MathInDegrees.Cos(halfAngle)),
            new Vector3(0, height, 0),
            new Vector3(-Range * MathInDegrees.Sin(halfAngle), height, Range * MathInDegrees.Cos(halfAngle)),
        });
    }

    void Update()
    {
        if (AnimationStartTime is {} startTime)
        {
            var percentComplete = Mathf.Min(1.0f, (Time.fixedTime - startTime) / AnimationDuration);
            RenderAngle(StartingAngle * (1.0f - percentComplete));
            if (percentComplete >= 1.0f)
            {
                NotifierImplementation.Notify(true);
                StopAnimation();
            }
        }
    }

    public IDisposable Subscribe(Action<bool> subscriber)
    {
        return NotifierImplementation.Subscribe(subscriber);
    }
}
