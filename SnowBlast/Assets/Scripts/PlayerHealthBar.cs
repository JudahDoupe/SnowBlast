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
            var fill = gameObject.transform.Find("Fill").GetComponent<Image>();
            var player = Find.ThePlayer;

            if (player == null)
            {
                slider.value = 0f;
            }
            else
            {
                var playerHealth = player.GetComponent<Health>();
                slider.value = 1.0f * playerHealth.CurrentHealth / playerHealth.MaxHealth;
            }
            
            if (slider.value < 0.3333f) fill.color = Color.red;
            else if (slider.value < 0.6666f) fill.color = Color.yellow;
            else fill.color = Color.green;

            Debug.Log($"{slider} {fill.color}");
        }
    }
}