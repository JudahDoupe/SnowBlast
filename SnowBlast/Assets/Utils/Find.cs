using UnityEngine;

namespace Assets.Utils
{
    public static class Find
    {
        public static GameObject ThePlayer => GameObject.FindGameObjectWithTag("Player");
    }
}