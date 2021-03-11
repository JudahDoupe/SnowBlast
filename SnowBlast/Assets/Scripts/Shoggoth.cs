using System;
using System.Collections.Generic;
using Assets.Utils;
using Assets.Utils.MyAnimator;
using UnityEngine;

public class Shoggoth : MonoBehaviour
{
    public float LurchStartSpeed = 1;
    public float LurchMinSpeed = 0.5f;
    public float LurchDuration = 2f;

    private float? LurchStartTime = null;

    private ParticleSystem SlimeTrail;

    private bool UncancellableLurch => LurchStartTime is {} t && Time.fixedTime - t < LurchDuration;

    // Start is called before the first frame update
    void Start()
    {
        SlimeTrail = transform.Find("SlimeTrail").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var player = Find.ThePlayer;
        if (!UncancellableLurch && player != null)
        {
            // Can rotate toward player
            var vec = transform.forward;
            var dir = player.transform.position - transform.position;
            var cross = Vector3.Cross(vec, dir);
            var angle = Vector3.Angle(vec, dir);

            // Rotate toward player
            transform.rotation = transform.rotation *
                                 Quaternion.AngleAxis(Mathf.Min(angle, 1) * Mathf.Sign(cross.y), Vector3.up);

            Debug.Log($"Angle {angle}");
            if (angle > 20)
            {
                Debug.Log($"Angle StopLurch");
                StopLurch();
                return;
            }
        }

        if (!UncancellableLurch)
        {
            if (player != null && Vector3.Distance(player.transform.position, transform.position) > 3)
            {
                StartLurch();
            }
            else
            {
                Debug.Log($"Distance StopLurch");
                StopLurch();
            }
        }
        else if (LurchStartTime is {} t)
        {
            var ratio = (Time.fixedTime - t) / LurchDuration;
            if (ratio > 1.0f)
            {
                StartLurch();
            }
            else
            {
                var speed = LurchStartSpeed - (LurchStartSpeed - LurchMinSpeed) * ratio;
                SetVelocity(speed);
            }
        }
    }

    public void StartLurch()
    {
        LurchStartTime = Time.fixedTime;
        var slimeTrailEmission = SlimeTrail.emission;
        slimeTrailEmission.enabled = true;
        SetVelocity(LurchStartSpeed);
    }

    public void StopLurch()
    {
        SetVelocity(0);
        var slimeTrailEmission = SlimeTrail.emission;
        slimeTrailEmission.enabled = false;
        LurchStartTime = null;
    }

    public void SetVelocity(float f)
    {
        gameObject.GetComponent<Rigidbody>()
            .velocity = transform.forward * f;
    }
}