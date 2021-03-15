using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Utils
{
    public static class Find
    {
        public static GameObject ThePlayer => GameObject.FindGameObjectWithTag("Player");

        public static PlayerState PlayerState => ThePlayer.GetComponent<PlayerState>();

        public static Quaternion CameraRotation = Quaternion.AngleAxis(45, Vector3.up);
    }
}