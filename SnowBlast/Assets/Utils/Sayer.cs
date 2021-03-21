using System;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Utils
{
    public static class Sayer
    {
        public static Action Say(GameObject subject, string directObject, bool confirmWithA)
        {
            var template = ObjectPool.Instance.Objects["SpeechBubble"];
            var instance = Object.Instantiate(template);
            var floater = instance.GetComponent<FloatAbove>();
            floater.SetTarget(subject);
            var textMesh = instance.transform.Find("Caption").GetComponent<TextMeshProUGUI>();
            textMesh.text = directObject;
            Find.TheCamera.Encompass(subject, instance);

            if (!confirmWithA)
            {
                var confirmText = instance.transform.Find("ConfirmWithA");
                confirmText.gameObject.SetActive(false);
            }

            return () =>
            {
                Find.TheCamera.Remove(subject, instance);
                Object.Destroy(instance);
            };
        }
    }
}