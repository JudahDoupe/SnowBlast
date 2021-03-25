﻿using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class Transition: MonoBehaviour
    {
        void Start()
        {
            BeginTransition(null);
        }

        public void BeginTransition(string nextScene)
        {
            var myPos = GetComponent<RectTransform>();
            var circle = transform.Find("Circle").GetComponent<RectTransform>();
            var circleEnd = transform.Find("CircleEnd").GetComponent<RectTransform>();
            var tail = transform.Find("Tail").GetComponent<RectTransform>();

            var uiSize = GameObject.Find("GUI").GetComponent<RectTransform>().rect;

            myPos.sizeDelta = new Vector2(uiSize.width, uiSize.height);
            myPos.localPosition = new Vector3(uiSize.width, 0, 0);

            circle.sizeDelta = new Vector2(uiSize.height, uiSize.height);

            tail.sizeDelta = new Vector2(uiSize.width, uiSize.height);
            tail.localPosition = new Vector3(uiSize.height / 2, 0, 0);

            circleEnd.sizeDelta = new Vector2(uiSize.height, uiSize.height);
            circleEnd.localPosition = new Vector3(uiSize.width, 0, 0);
            
            Debug.Log(uiSize.width + uiSize.height);


            this.BeginSerial()
                .Then(1.0f,
                    new Vector3(uiSize.width, 0, 0),
                    new Vector3(-uiSize.width - uiSize.height, 0, 0),
                    r =>
                    {
                        myPos.localPosition = r;
                    })
                .Start(() => BeginTransition(null));
        }
    }
}