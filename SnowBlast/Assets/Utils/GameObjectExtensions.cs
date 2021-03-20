using System;
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

        public static Bounds GetBounds(this GameObject g) => GetBounds(g.transform);

        public static Bounds GetBounds(this MonoBehaviour g) => GetBounds(g.transform);

        public static Bounds GetBounds(this Transform g)
        {
            var rect = g.GetComponent<RectTransform>();

            if (rect != null)
            {
                var uiScaleFactor = 1.0f;
                var bounds = new Bounds(rect.position, new Vector3(rect.rect.width, rect.rect.height, 0.0f) * uiScaleFactor);

                if (rect.childCount > 0)
                {
                    foreach (RectTransform child in rect)
                    {
                        Bounds childBounds = new Bounds(child.position, new Vector3(child.rect.width, child.rect.height, 0.0f) * uiScaleFactor);
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
    }
}