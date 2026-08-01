using UnityEngine;
using ShadowEscape.Player;
using ShadowEscape.Managers;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// When the player touches this checkpoint, it becomes their new respawn point
    /// and the game auto-saves progress.
    /// Attach to: Checkpoint GameObject with Collider2D (Is Trigger).
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private ParticleSystem activateParticles;
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private string checkpointId = "checkpoint_1";

        private bool activated;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && inactiveSprite != null) spriteRenderer.sprite = inactiveSprite;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (activated || !other.CompareTag("Player")) return;

            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.SetCheckpoint(transform.position);
            }

            activated = true;
            if (spriteRenderer != null && activeSprite != null) spriteRenderer.sprite = activeSprite;
            if (activateParticles != null) activateParticles.Play();

            GameManager.Instance?.SaveCheckpoint(checkpointId, transform.position);
        }
    }
}
