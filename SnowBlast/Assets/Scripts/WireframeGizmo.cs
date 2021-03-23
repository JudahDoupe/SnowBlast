using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class WireframeGizmo : MonoBehaviour
    {
        void OnDrawGizmosSelected()
        {
            var bounds = this.GetBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}