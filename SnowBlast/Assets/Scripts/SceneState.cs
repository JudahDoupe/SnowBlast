using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class SceneState : MonoBehaviour
    {
        public bool WeaponsFree = true;

        void Start()
        {
            if (!WeaponsFree)
            {
                Find.PlayerState.WeaponsBlocker.Block(this);
            }
        }
    }
}