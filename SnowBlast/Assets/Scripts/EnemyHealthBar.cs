using Assets.Utils;
using FluentAssertions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EnemyHealthBar : MonoBehaviour
    {
        public void SetParent(GameObject parent)
        {
            var targetHealth = parent.GetComponent<Health>();
            targetHealth.Should().NotBeNull("EnemyHealthBar passed a gameobject without a Health component");
            targetHealth.Subscribe(UpdateHealthBar);

            var parentObjectHeight = GetMaxBounds(parent).max.y;
            var myHeight = gameObject.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
            
            Debug.Log($"{parentObjectHeight} + {myHeight} = {parentObjectHeight + myHeight}");

            gameObject.transform.SetParent(parent.transform);
            gameObject.transform.localPosition = new Vector3(0, parentObjectHeight + myHeight + 1, 0);
        }

        public void Update()
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, -Camera.main.transform.up);
        }

        void UpdateHealthBar(HealthNotification healthNotification)
        {
            var slider = GetComponent<Slider>();
            slider.value = 1.0f * healthNotification.CurrentHealth / healthNotification.MaxHealth;
        }

        Bounds GetMaxBounds(GameObject g)
        {
            var b = new Bounds(g.transform.position, Vector3.zero);
            foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }
    }
}