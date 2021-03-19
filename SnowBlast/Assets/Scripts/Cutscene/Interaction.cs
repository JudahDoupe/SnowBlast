using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Cutscene
{
    public class Interaction : MonoBehaviour
    {
        public string InteractionPrompt = "Interact";

        public TextAsset CutsceneAsset;

        private readonly BlockerStack InputBlocker = new BlockerStack();

        void Start()
        {
            var input = GetComponent<PlayerInput>();
            InputBlocker.Subscribe(state =>
            {
                if (state == BlockerStack.BlockState.Blocked)
                {
                    GameObject.Find("GUI").transform.Find("InteractionPrompt").gameObject.SetActive(false);
                    input.DeactivateInput();
                }
                else
                {
                    GameObject.Find("GUI").transform.Find("InteractionPrompt").gameObject.SetActive(true);
                    input.ActivateInput();
                }
            });
            InputBlocker.Block(this);
        }

        void Update()
        {
            var player = Find.ThePlayer;

            if (player == null) return;

            var intersection = this.GetMaxBounds().Intersects(player.GetMaxBounds());
            if (intersection)
            {
                InputBlocker.Unblock(this);
            }
            else
            {
                InputBlocker.Block(this);
            }
        }

        void OnConfirm()
        {
            Debug.Log("Hello world!");
            var unblock = Find.PlayerState.BlockAll();
            var animation = new CutsceneAnimator(CutsceneAsset).Create();
            StartCoroutine(animation.Start(() => unblock()));
        }
    }
}
