using Assets.Scripts;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Utils
{
    public static class Find
    {
        public static GameObject ThePlayer => GameObject.FindGameObjectWithTag("Player");

        public static PlayerState PlayerState => ThePlayer.GetComponent<PlayerState>();

        public static SceneState SceneState
        {
            get
            {
                var st = GameObject.FindGameObjectWithTag("SceneState");
                if (st != null) return st.GetComponent<SceneState>();
                return new SceneState();
            }
        }

        public static Quaternion CameraRotation = Quaternion.AngleAxis(45, Vector3.up);
    }
}