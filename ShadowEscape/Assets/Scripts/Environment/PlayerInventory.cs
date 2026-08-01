using System.Collections.Generic;
using UnityEngine;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// Tracks which keys the player currently holds.
    /// Attach to: Player GameObject.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private readonly HashSet<string> keys = new HashSet<string>();

        public void AddKey(string keyId) => keys.Add(keyId);
        public bool HasKey(string keyId) => keys.Contains(keyId);
        public void UseKey(string keyId) => keys.Remove(keyId); // remove if keys are single-use

        public IReadOnlyCollection<string> Keys => keys;
    }
}
