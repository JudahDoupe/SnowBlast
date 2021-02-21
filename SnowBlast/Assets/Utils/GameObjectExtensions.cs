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

        public static T UltimateParent<T>(this MonoBehaviour self)
        {
            var parent = self.transform.parent;
            while (parent != null)
            {
                if (self.transform.parent is T t) return t;
                parent = self.transform.parent;
            }

            throw new ApplicationException($"No parent implements {typeof(T).Name}");
        }
    }
}