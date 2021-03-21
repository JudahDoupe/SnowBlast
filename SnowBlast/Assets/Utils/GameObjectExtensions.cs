using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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

        public static Bounds GetMaxBounds(this IEnumerable<GameObject> self)
        {
            var items = self.ToList();
            var bounds = items.First().GetBounds();
            foreach (var go in items.Skip(1))
            {
                bounds.Encapsulate(go.GetBounds());
            }

            return bounds;
        }

        public static Bounds GetBounds(this GameObject g) => GetBounds(g.transform);

        public static Bounds GetBounds(this MonoBehaviour g) => GetBounds(g.transform);

        public static Bounds GetBounds(this Transform g)
        {
            var rect = g.GetComponent<RectTransform>();

            if (rect != null)
            {
                var uiScaleFactor = rect.transform.lossyScale;
                var bounds = new Bounds(rect.position, new Vector3(rect.rect.width * uiScaleFactor.x, 
                    rect.rect.height * uiScaleFactor.y, 0.0f));

                if (rect.childCount > 0)
                {
                    foreach (RectTransform child in rect)
                    {
                        Bounds childBounds = new Bounds(child.position, new Vector3(child.rect.width * uiScaleFactor.x, 
                            child.rect.height * uiScaleFactor.y, 0.0f));
                        bounds.Encapsulate(childBounds);
                    }
                }

                return bounds;
            }


            var renderers = g.GetComponentsInChildren<Renderer>()
                .Where(it => it.gameObject.GetComponent<ParticleSystem>() == null)
                .ToList();
            renderers.Should().NotBeEmpty("Attempt to get null bounds!");
            var first = renderers.First();
            var b = new Bounds(first.bounds.center, first.bounds.size);
            foreach (var r in renderers.Skip(1))
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }

        public static IEnumerable<Vector3> Corners(this Bounds bounds)
        {
            var pmin = bounds.min;
            var pmax = bounds.max;

            foreach (var x in new[] {pmin.x, pmax.x})
            {
                foreach (var y in new[] {pmin.y, pmax.y})
                {
                    foreach (var z in new[] {pmin.z, pmax.z})
                    {
                        yield return new Vector3(x, y, z);
                    }
                }
            }
        }
    }
}