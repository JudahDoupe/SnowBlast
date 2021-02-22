using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Utils;
using Time = UnityEngine.Time;

public class WaveAnnouncement : MonoBehaviour
{
    enum AnimationState
    {
        SlidingIn,
        Centered,
        SlidingOut
    }

    private AnimationState State;
    private float LingerTimeout;
    private float SlideStartTime;
    public float Linger = 1.5f;
    public float SlideTime = 1.5f;
    public float TotalTime => Linger + 2 * SlideTime;

    private string Text
    {
        get => GetComponent<Text>().text;
        set => GetComponent<Text>().text = value;
    }

    private float UiLayerWidth => gameObject.transform.parent.GetComponent<RectTransform>().rect.width;

    void Update()
    {
        var current = gameObject.transform.position;
        

        switch (State)
        {
            case AnimationState.SlidingIn:
            {
                var rect = GetComponent<RectTransform>().rect;
                var newX = SlideLeft(UiLayerWidth / 2, UiLayerWidth + rect.width / 2);

                gameObject.transform.position = new Vector3(newX, current.y, current.z);

                if (gameObject.transform.position.x <= UiLayerWidth / 2)
                {
                    current.x = UiLayerWidth / 2;
                    State = AnimationState.Centered;
                    LingerTimeout = Time.fixedTime + Linger;
                }

                break;
            }
            case AnimationState.Centered:
                if (Time.fixedTime >= LingerTimeout)
                {
                    State = AnimationState.SlidingOut;
                    SlideStartTime = Time.fixedTime;
                }
                break;
            default:
            {
                var rect = GetComponent<RectTransform>().rect;
                var newX = SlideLeft(-rect.width / 2, UiLayerWidth / 2);
                gameObject.transform.position = new Vector3(newX, current.y, current.z);
                if (gameObject.transform.position.x <= -(rect.width/2))
                {
                    gameObject.SetActive(false);
                }

                break;
            }
        }
    }

    public void Say(string text)
    {
        State = AnimationState.SlidingIn;
        Text = text;
        SlideStartTime = Time.fixedTime;
        var current = gameObject.transform.position;
        gameObject.transform.position = new Vector3(UiLayerWidth, current.y, current.z);
        gameObject.SetActive(true);
    }

    private float SlideLeft(float targetX, float startX)
    {
        var elapsed = Time.fixedTime - SlideStartTime;
        return targetX + (1.0f - Math.Min(1.0f, elapsed / SlideTime)) * (startX - targetX);
    }
}
