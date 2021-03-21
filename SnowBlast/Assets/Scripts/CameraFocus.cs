using System.Linq;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraFocus: MonoBehaviour
    {
        private Bounds last = new Bounds();
        void FixedUpdate()
        {
            var encompassed = Find.TheCamera.GetEncompassed();

            if (!encompassed.Any()) return;

            var b = encompassed.GetMaxBounds();

            if (b.center == last.center && b.size == last.size) return;
            last = b;
            transform.position = b.center;
            transform.localScale = b.size;
        }
    }
}