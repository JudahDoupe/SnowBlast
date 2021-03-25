using System;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Cutscenes;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public interface IInteraction
    {
        void Play();
    }

    public class InteractionAction: IInteraction
    {
        private readonly Action Action;

        public InteractionAction(Action action)
        {
            Action = action;
        }

        public void Play()
        {
            Action();
        }
    }

    public class Interaction : MonoBehaviour, IInteraction
    {
        public string InteractionPrompt = "Interact";

        public TextAsset CutsceneAsset;

        private bool Done;

        void Update()
        {
            if (Done) return;
            var player = Find.ThePlayer;

            if (player == null) return;

            if (PlayerIntersected())
            {
                Find.SceneState.SetInteraction(this, true);
            }
            else
            {
                Find.SceneState.ClearInteraction(this);
            }
        }

        private bool PlayerIntersected()
        {
            var player = Find.ThePlayer;
            return player != null && this.GetBounds().Intersects(player.GetBounds());
        }

        public void Play()
        {
            Done = true;
            var unblock = Find.PlayerState.BlockAll();
            var animation = CutsceneParser.Parse(CutsceneAsset.text);
            StartCoroutine(animation.Begin(() => unblock()));
        }
    }
}
