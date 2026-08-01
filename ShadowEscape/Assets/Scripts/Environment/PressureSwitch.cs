using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ShadowEscape.Managers;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// A floor switch that is "pressed" while the player, a shadow clone, or a
    /// pushable box stands on it. Fires UnityEvents so it can open doors,
    /// move platforms, etc. entirely from the Inspector -- no extra code needed
    /// per puzzle.
    /// Attach to: Switch GameObject with a Collider2D set to "Is Trigger".
    /// </summary>
    public class PressureSwitch : MonoBehaviour
    {
        [SerializeField] private string[] acceptedTags = { "Player", "ShadowClone", "Pushable" };
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private Sprite unpressedSprite;
        [SerializeField] private ParticleSystem activateParticles;

        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        private readonly HashSet<Collider2D> objectsOnSwitch = new HashSet<Collider2D>();
        private SpriteRenderer spriteRenderer;
        public bool IsPressed => objectsOnSwitch.Count > 0;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsAccepted(other)) return;

            bool wasPressed = IsPressed;
            objectsOnSwitch.Add(other);

            if (!wasPressed && IsPressed)
            {
                Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsAccepted(other)) return;

            objectsOnSwitch.Remove(other);

            if (!IsPressed)
            {
                Deactivate();
            }
        }

        private bool IsAccepted(Collider2D other)
        {
            foreach (var t in acceptedTags)
            {
                if (other.CompareTag(t)) return true;
            }
            return false;
        }

        private void Activate()
        {
            if (spriteRenderer != null && pressedSprite != null) spriteRenderer.sprite = pressedSprite;
            if (activateParticles != null) activateParticles.Play();
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            OnPressed?.Invoke();
        }

        private void Deactivate()
        {
            if (spriteRenderer != null && unpressedSprite != null) spriteRenderer.sprite = unpressedSprite;
            OnReleased?.Invoke();
        }
    }
}
