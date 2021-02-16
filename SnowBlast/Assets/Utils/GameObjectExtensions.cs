using UnityEngine;

namespace Assets.Utils
{
    public static class GameObjectExtensions
    {
        public static GameObject GetUltimateParent(this GameObject self)
        {
            while (self.transform.parent != null)
            {
                self = self.transform.parent.gameObject;
            }

            return self;
        }
    }
}