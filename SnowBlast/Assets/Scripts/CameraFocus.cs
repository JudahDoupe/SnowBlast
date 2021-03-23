using System.Linq;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraFocus: MonoBehaviour
    {
        void OnDrawGizmosSelected()
        {
            var encompassed = Find.TheCamera.GetEncompassed();

            foreach (var go in encompassed)
            {
                var bounds = go.GetBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }

            var b = encompassed.GetMaxBounds();
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}