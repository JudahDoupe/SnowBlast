using System;
using UnityEngine;

namespace Assets.Utils
{
    public static class GameObjectExtensions
    {
        public static GameObject UltimateParent(this GameObject self)
        {
            while (self.transform.parent != null)
            {
                self = self.transform.parent.gameObject;
            }

            return self;
        }

        public static Bounds GetMaxBounds(this GameObject g) => GetMaxBounds(g.transform);

        public static Bounds GetMaxBounds(this MonoBehaviour g) => GetMaxBounds(g.transform);

        public static Bounds GetMaxBounds(this Transform g)
        {
            var b = new Bounds(g.transform.position, Vector3.zero);
            foreach (var r in g.GetComponentsInChildren<Renderer>())
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }
    }
}