using System;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Utils
{
    public static class Sayer
    {
        public static Action Say(GameObject subject, string directObject)
        {
            var speechBubbleTemplate = ObjectPool.Instance.Objects["SpeechBubble"];
            var speechBubbleInstance = Object.Instantiate(speechBubbleTemplate);
            var floater = speechBubbleInstance.GetComponent<FloatAbove>();
            floater.SetTarget(subject);
            var textMesh = speechBubbleInstance.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = directObject;
            Find.TheCamera.Encompass(subject, speechBubbleInstance);
            return () =>
            {
                Find.TheCamera.Remove(subject, speechBubbleInstance);
                Object.Destroy(speechBubbleInstance);
            };
        }
    }
}