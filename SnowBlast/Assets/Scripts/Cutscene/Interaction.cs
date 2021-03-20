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

        private readonly LogicalOrSet InputBlocked = new LogicalOrSet();

        private bool Done;

        void Start()
        {
            var input = GetComponent<PlayerInput>();

            Find.SceneState.InteractionPromptShown.Subscribe(shown =>
            {
                if (shown && PlayerIntersected()) InputBlocked.Remove(this);
                else InputBlocked.Add(this);
            });

            InputBlocked.Subscribe(inputBlocked =>
            {
                if (inputBlocked)
                {
                    input.DeactivateInput();
                }
                else
                {
                    input.ActivateInput();
                }
            });
            InputBlocked.Add(this);
        }

        void Update()
        {
            if (Done) return;
            var player = Find.ThePlayer;

            if (player == null) return;

            if (PlayerIntersected())
            {
                Find.SceneState.InteractionPromptShown.Add(this);
            }
            else
            {
                Find.SceneState.InteractionPromptShown.Remove(this);
            }
        }

        private bool PlayerIntersected()
        {
            var player = Find.ThePlayer;
            return player != null && this.GetBounds().Intersects(player.GetBounds());
        }

        void OnConfirm()
        {
            Find.SceneState.InteractionPromptShown.Remove(this);
            Done = true;
            InputBlocked.Add("Done");
            var unblock = Find.PlayerState.BlockAll();
            var animation = new CutsceneAnimator(CutsceneAsset).Create();
            StartCoroutine(animation.Begin(() => unblock()));
        }
    }
}
