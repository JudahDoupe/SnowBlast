using UnityEngine;

namespace Assets.Utils
{
    public static class Find
    {
        public static GameObject ThePlayer => GameObject.FindGameObjectWithTag("Player");

        public static Quaternion CameraRotation = Quaternion.AngleAxis(45, Vector3.up);
    }
}