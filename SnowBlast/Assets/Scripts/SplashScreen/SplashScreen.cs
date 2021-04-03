using System.Linq;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.SplashScreen
{
    public class SplashScreen : MonoBehaviour
    {
        void Start()
        {
            var logoMaterial = transform.Find("Logo").GetComponent<Image>();
            var keyText = transform.Find("AnyKey").GetComponent<Text>();
            keyText.SetAlpha(0.0f);
            keyText.SetAlpha(0.0f);

            var showAnyKey = FlashAnyKey(keyText);

            // Show logo
            ShowLogo(logoMaterial, showAnyKey);
        }

        void Update()
        {
            if (
                (Gamepad.current is {} gamepad &&
                    gamepad.allControls.Any(x => x is ButtonControl button && button.isPressed && !x.synthetic))
                || Keyboard.current?.anyKey.isPressed == true)
            {
                SceneManager.LoadScene("Hallway");
            }
        }

        private void ShowLogo(Graphic logoMaterial, StartableTweener showAnyKey)
        {
            this.BeginSerial()
                .FadeIn(2.0f, logoMaterial)
                .Wait(0.5f)
                .Then(() => showAnyKey.Start())
                .Start();
        }

        private StartableTweener FlashAnyKey(Text keyText)
        {
            return this.BeginSerial()
                .FadeIn(1.0f, keyText)
                .FadeOut(1.0f, keyText)
                .Then(() => FlashAnyKey(keyText).Start());
        }

        
    }
}