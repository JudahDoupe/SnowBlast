using Assets.Utils;
using Assets.Utils.JBehavior;
using UnityEngine;

namespace Assets.Scripts.Cutscene
{
    public class Interaction : MonoBehaviour
    {
        public string InteractionPrompt = "Interact";

        public TextAsset CutsceneAsset;

        private bool CurrentState = false;

        void Update()
        {
            var player = Find.ThePlayer;

            if (player == null) return;

            CurrentState = this.GetMaxBounds().Intersects(player.GetMaxBounds());

            GameObject.Find("GUI").transform.Find("InteractionPrompt").gameObject.SetActive(CurrentState);
        }

        void OnPrimaryAttack()
        {
            if (CurrentState && !Find.PlayerState.InputBlocker.Blocked)
            {
                var unblock = Find.PlayerState.BlockAll();
                JBehaviorSet animation = new CutsceneAnimator(CutsceneAsset).Create();
                StartCoroutine(animation.Start(() => unblock()));
            }
        }
    }
}
