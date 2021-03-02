﻿using UnityEngine;

namespace Assets.Utils
{
    public static class VectorExtensions
    {
        public static Vector2 XZ(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        public static Vector3 XZ(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }
    }
}