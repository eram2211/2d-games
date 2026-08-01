using System;
using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Player
{
    /// <summary>
    /// Tracks player health, handles damage, death and respawn at the last checkpoint.
    /// Attach to: Player GameObject.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float invulnerabilityDuration = 1f;
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] private ParticleSystem deathParticles;

        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; private set; }
        public event Action<int, int> OnHealthChanged; // current, max
        public event Action OnPlayerDied;

        private bool isInvulnerable;
        private Vector3 lastCheckpointPosition;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            CurrentHealth = maxHealth;
            lastCheckpointPosition = transform.position;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (isInvulnerable || CurrentHealth <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (hitParticles != null) hitParticles.Play();

            if (CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvulnerabilityFlash());
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void Die()
        {
            if (deathParticles != null) deathParticles.Play();
            OnPlayerDied?.Invoke();
            GameManager.Instance?.OnPlayerDeath();
        }

        /// <summary>Called by GameManager after showing the lose screen / respawn delay.</summary>
        public void RespawnAtCheckpoint()
        {
            transform.position = lastCheckpointPosition;
            CurrentHealth = maxHealth;
            if (rb != null) rb.velocity = Vector2.zero;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void SetCheckpoint(Vector3 position)
        {
            lastCheckpointPosition = position;
        }

        private System.Collections.IEnumerator InvulnerabilityFlash()
        {
            isInvulnerable = true;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            float elapsed = 0f;
            while (elapsed < invulnerabilityDuration)
            {
                if (sr != null) sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }
            if (sr != null) sr.enabled = true;
            isInvulnerable = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Hazard"))
            {
                TakeDamage(25);
            }
        }
    }
}
