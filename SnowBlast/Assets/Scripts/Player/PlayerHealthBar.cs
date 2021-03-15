using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class PlayerHealthBar : MonoBehaviour
    {
        void Start()
        {
            Find.ThePlayer.GetComponent<Health>()
                .Subscribe(UpdateHealthBar);
        }

        void UpdateHealthBar(HealthNotification healthNotification)
        {
            var slider = GetComponent<Slider>();
            var fill = gameObject.transform.Find("Fill").GetComponent<Image>();
            slider.value = 1.0f * healthNotification.CurrentHealth / healthNotification.MaxHealth;
            
            if (slider.value < 0.3333f) fill.color = Color.red;
            else if (slider.value < 0.6666f) fill.color = Color.yellow;
            else fill.color = Color.green;
        }
    }
}