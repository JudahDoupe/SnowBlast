#nullable enable
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scene
{
    public class Transition : MonoBehaviour
    {
        void Start()
        {
            EndTransition();
        }

        public void StartTransition(string nextScene) => Transit(StartPosition, MidPosition, nextScene);
        private void EndTransition() => Transit(MidPosition, EndPosition, null);

        private void Transit(Vector3 start, Vector3 end, string? nextScene)
        {
            Resize();
            var myPos = GetComponent<RectTransform>();

            this.BeginSerial()
                .Then(1.0f,
                    start,
                    end,
                    r => { myPos.localPosition = r; })
                .Start(() =>
                {
                    if (nextScene != null)
                    {
                        SceneManager.LoadScene(nextScene);
                    }
                });
        }

        private Rect UiSize => GameObject.Find("GUI").GetComponent<RectTransform>().rect;

        private Vector3 StartPosition => new Vector3(UiSize.width, 0, 0);
        private Vector3 MidPosition => new Vector3(-UiSize.height / 2, 0, 0);
        private Vector3 EndPosition => new Vector3((-UiSize.height) - UiSize.width, 0, 0);

        private void Resize()
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
        }
    }
}