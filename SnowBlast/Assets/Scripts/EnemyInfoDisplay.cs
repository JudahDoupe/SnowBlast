using Assets.Utils;
using FluentAssertions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EnemyInfoDisplay : MonoBehaviour
    {
        private Slider HealthBarSlider => gameObject.transform.Find("EnemyHealthBar").GetComponent<Slider>();
        private Transform LockOnIndicator => gameObject.transform.Find("LockOnIndicator");
        private int LockOnRotationSpeed = 2;

        public void SetParent(GameObject parent)
        {
            var targetHealth = parent.GetComponent<Health>();
            targetHealth.Should().NotBeNull("EnemyHealthBar passed a gameobject without a Health component");
            targetHealth.Subscribe(UpdateHealthBar);

            var parentObjectHeight = parent.GetMaxBounds().max.y;
            var myHeight = gameObject.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
            
            gameObject.transform.SetParent(parent.transform);
            gameObject.transform.localPosition = new Vector3(0, parentObjectHeight + myHeight + 1, 0);
        }

        public void StartLockOn()
        {
            LockOnIndicator.gameObject.SetActive(true);
        }

        public void StopLockOn()
        {
            LockOnIndicator.gameObject.SetActive(false);
        }

        public void Update()
        {
            LockOnIndicator.Rotate(Vector3.up * LockOnRotationSpeed);
        }

        void UpdateHealthBar(HealthNotification healthNotification)
        {
            HealthBarSlider.value = 1.0f * healthNotification.CurrentHealth / healthNotification.MaxHealth;
        }
    }
}