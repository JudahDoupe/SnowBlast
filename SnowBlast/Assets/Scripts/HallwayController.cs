using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Cutscenes;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

namespace Assets.Scripts
{
    public class HallwayController : MonoBehaviour
    {
        [SerializeField]
        private TextAsset Cutscene;

        void Start()
        {
            if (GameState.ArenaVictory)
            {
                StartCoroutine(CutsceneParser.Parse(Cutscene.text)
                    .Begin());
            }
        }
    }
}