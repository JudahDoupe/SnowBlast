#nullable enable
using Assets.Scripts.Cutscene;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneState : MonoBehaviour
    {
        public bool WeaponsFree = true;

        private IInteraction? InteractionObject;

        public void SetInteraction(IInteraction interactionObject, bool showInteractionPrompt)
        {
            InteractionObject = interactionObject;
            if (showInteractionPrompt)
            {
                SetInteractionPrompt(true);
            }
        }

        public void ClearInteraction(IInteraction interactionObject)
        {
            if (InteractionObject == interactionObject)
            {
                InteractionObject = null;
                SetInteractionPrompt(false);
            }
        }

        private void SetInteractionPrompt(bool shown)
        {
            GameObject.Find("GUI").transform.Find("InteractionPrompt").gameObject.SetActive(shown);
        }

        void Start()
        {
            if (!WeaponsFree)
            {
                Find.PlayerState.WeaponsBlocked.Add(this);
            }
        }

        void OnConfirmInteraction()
        {
            SetInteractionPrompt(false);
            var original = InteractionObject;
            InteractionObject = null;
            original?.Play();
        }
    }
}