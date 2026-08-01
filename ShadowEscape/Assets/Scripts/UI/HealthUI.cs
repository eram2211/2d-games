using UnityEngine;
using UnityEngine.UI;
using ShadowEscape.Player;

namespace ShadowEscape.UI
{
    /// <summary>
    /// Updates a UI Slider (or Image with fillAmount) to reflect the player's
    /// current health. Subscribes to PlayerHealth.OnHealthChanged.
    /// Attach to: HealthBar GameObject inside the level's Canvas.
    /// </summary>
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;

        private void Start()
        {
            var playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
        }

        private void UpdateHealthBar(int current, int max)
        {
            if (healthSlider == null) return;
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }
    }
}
