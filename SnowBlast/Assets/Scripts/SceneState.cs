using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneState : MonoBehaviour
    {
        public bool WeaponsFree = true;

        [HideInInspector]
        public readonly LogicalOrSet InteractionPromptShown = new LogicalOrSet();

        void Start()
        {
            if (!WeaponsFree)
            {
                Find.PlayerState.WeaponsBlocked.Add(this);
            }

            InteractionPromptShown.Subscribe(shown =>
            {
                GameObject.Find("GUI").transform.Find("InteractionPrompt").gameObject.SetActive(shown);
            });
        }
    }
}