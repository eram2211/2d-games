using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// Marks the goal/exit of a level. When the player reaches it, triggers
    /// the win flow via GameManager.
    /// Attach to: Exit GameObject with Collider2D (Is Trigger).
    /// </summary>
    public class LevelExit : MonoBehaviour
    {
        [SerializeField] private ParticleSystem exitParticles;
        private bool triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered || !other.CompareTag("Player")) return;
            triggered = true;

            if (exitParticles != null) exitParticles.Play();
            GameManager.Instance?.CompleteLevel();
        }
    }
}
