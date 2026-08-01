using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowEscape.Managers
{
    /// <summary>
    /// Plain data container that gets serialized to JSON for save/load.
    /// Not attached to any GameObject -- used by SaveSystem.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int highestLevelUnlocked = 0;
        public List<CheckpointEntry> checkpoints = new List<CheckpointEntry>();
        public float musicVolume = 0.5f;
        public float sfxVolume = 0.8f;

        [Serializable]
        public class CheckpointEntry
        {
            public string sceneName;
            public float x, y, z;
        }

        public bool HasCheckpointForLevel(string sceneName)
        {
            return checkpoints.Exists(c => c.sceneName == sceneName);
        }

        public Vector3 GetCheckpointForLevel(string sceneName)
        {
            var entry = checkpoints.Find(c => c.sceneName == sceneName);
            return entry != null ? new Vector3(entry.x, entry.y, entry.z) : Vector3.zero;
        }

        public void SetCheckpointForLevel(string sceneName, Vector3 position)
        {
            var entry = checkpoints.Find(c => c.sceneName == sceneName);
            if (entry == null)
            {
                entry = new CheckpointEntry { sceneName = sceneName };
                checkpoints.Add(entry);
            }
            entry.x = position.x;
            entry.y = position.y;
            entry.z = position.z;
        }
    }
}
