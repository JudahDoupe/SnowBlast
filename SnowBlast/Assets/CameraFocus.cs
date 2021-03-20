using System.Linq;
using Assets.Utils;
using UnityEngine;

namespace Assets
{
    public class CameraFocus: MonoBehaviour
    {
        private Bounds last = new Bounds();
        void FixedUpdate()
        {
            var encompassed = Find.TheCamera.GetEncompassed();

            if (!encompassed.Any()) return;

            var b = encompassed.First().GetBounds();
            foreach(var item in encompassed.Skip(1)) b.Encapsulate(item.GetBounds());
            if (b.center == last.center && b.size == last.size) return;
            last = b;
            Debug.Log($"{b.center} {b.size}");
            transform.position = b.center;
            transform.localScale = b.size;
        }
    }
}