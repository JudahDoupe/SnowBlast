using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerHealthBar : MonoBehaviour
    {
        void Update()
        {
            
            var slider = GetComponent<Slider>();
            var player = Find.ThePlayer;

            if (player == null)
            {
                slider.value = 0f;
                return;
            }

            var playerHealth = player.GetComponent<Health>();
            slider.value = 1.0f * playerHealth.CurrentHealth / playerHealth.MaxHealth;
        }
    }
}