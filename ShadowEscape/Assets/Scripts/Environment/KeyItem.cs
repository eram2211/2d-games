using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// A pickup that grants the player a key with the given id, matched against
    /// a Door's requiredKeyId.
    /// Attach to: Key GameObject with Collider2D (Is Trigger).
    /// </summary>
    public class KeyItem : MonoBehaviour
    {
        [SerializeField] private string keyId = "gold_key";
        [SerializeField] private ParticleSystem pickupParticles;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey(keyId);
                if (pickupParticles != null)
                {
                    var fx = Instantiate(pickupParticles, transform.position, Quaternion.identity);
                    fx.Play();
                    Destroy(fx.gameObject, 2f);
                }
                AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
                Destroy(gameObject);
            }
        }
    }
}
