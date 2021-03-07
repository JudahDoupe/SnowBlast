using UnityEngine;

namespace Assets.Utils
{
    public static class MathInDegrees
    {
        public static float Cos(float angleInDegrees) => Mathf.Cos(Mathf.Deg2Rad * angleInDegrees);
        public static float Sin(float angleInDegrees) => Mathf.Sin(Mathf.Deg2Rad * angleInDegrees);
    }
}