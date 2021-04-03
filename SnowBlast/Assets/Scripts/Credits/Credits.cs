using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Credits
{
    public class Credits : MonoBehaviour
    {
        void Start()
        {
            var aGameBy = transform.Find("AGameBy").GetComponent<TextMeshProUGUI>();
            var zatarains = transform.Find("Zatarains").GetComponent<TextMeshProUGUI>();
            var names = transform.Find("Names").GetComponent<TextMeshProUGUI>();
            var thankYou = transform.Find("ThankYou").GetComponent<TextMeshProUGUI>();

            aGameBy.SetAlpha(0.0f);
            zatarains.SetAlpha(0.0f);
            names.SetAlpha(0.0f);
            thankYou.SetAlpha(0.0f);

            this.BeginSerial()
                .FadeIn(1.0f, aGameBy)
                .FadeIn(1.0f, zatarains)
                .FadeIn(1.0f, names)
                .Wait(2.0f)
                .Append(new ParallelTweener()
                    .FadeOut(1.0f, aGameBy)
                    .FadeOut(1.0f, zatarains)
                    .FadeOut(1.0f, names))
                .FadeIn(1.0f, thankYou)
                .Start();
        }
    }
}