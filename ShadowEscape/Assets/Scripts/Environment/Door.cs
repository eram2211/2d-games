using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// A door that can be opened either by a matching key (via KeyItem/PlayerInventory)
    /// or by a linked PressureSwitch (hook OnPressed/OnReleased in the Inspector to
    /// Open()/Close()). Plays door-open audio and disables its collider when open.
    /// Attach to: Door GameObject with Collider2D + Animator (optional) + AudioSource.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [SerializeField] private string requiredKeyId = ""; // leave empty if switch-controlled only
        [SerializeField] private bool startsOpen = false;
        [SerializeField] private Animator animator; // optional, plays "Open"/"Close" triggers
        [SerializeField] private ParticleSystem openParticles;

        private Collider2D col;
        public bool IsOpen { get; private set; }

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            IsOpen = startsOpen;
            ApplyState(instant: true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsOpen || string.IsNullOrEmpty(requiredKeyId)) return;
            if (!other.CompareTag("Player")) return;

            var inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasKey(requiredKeyId))
            {
                inventory.UseKey(requiredKeyId);
                Open();
            }
        }

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            ApplyState(instant: false);
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.doorOpenClip);
            if (openParticles != null) openParticles.Play();
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            ApplyState(instant: false);
        }

        private void ApplyState(bool instant)
        {
            if (col != null) col.enabled = !IsOpen;
            if (animator != null) animator.SetTrigger(IsOpen ? "Open" : "Close");
        }
    }
}
