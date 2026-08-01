using System.Collections.Generic;
using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Player
{
    /// <summary>
    /// Core shadow mechanic: records the player's position/state for a set duration,
    /// then spawns a "shadow clone" that replays those exact movements, so it can
    /// stand on pressure switches, block hazards, etc. while the real player moves on.
    /// Attach to: Player GameObject.
    /// </summary>
    public class ShadowClone : MonoBehaviour
    {
        [Header("Recording")]
        [SerializeField] private float recordDuration = 8f;
        [SerializeField] private float sampleInterval = 0.04f;
        [SerializeField] private int maxActiveClones = 1;

        [Header("Clone Prefab")]
        [SerializeField] private GameObject clonePrefab; // simple sprite + collider that follows recorded frames
        [SerializeField] private KeyCode spawnKey = KeyCode.E;

        private struct Frame
        {
            public Vector3 position;
            public bool facingRight;
            public float timestamp;
        }

        private readonly List<Frame> recordedFrames = new List<Frame>();
        private readonly List<GameObject> activeClones = new List<GameObject>();
        private float recordTimer;
        private bool isRecording;
        private PlayerController playerController;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            BeginRecording();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

            if (isRecording)
            {
                RecordFrame();
            }

            if (Input.GetKeyDown(spawnKey) && recordedFrames.Count > 1)
            {
                SpawnClone();
            }
        }

        private void BeginRecording()
        {
            recordedFrames.Clear();
            recordTimer = 0f;
            isRecording = true;
        }

        private void RecordFrame()
        {
            recordTimer += Time.deltaTime;
            if (recordedFrames.Count == 0 || recordTimer - recordedFrames[recordedFrames.Count - 1].timestamp >= sampleInterval)
            {
                recordedFrames.Add(new Frame
                {
                    position = transform.position,
                    facingRight = playerController != null && playerController.FacingRight,
                    timestamp = recordTimer
                });
            }

            // Keep only the last `recordDuration` seconds of frames
            while (recordedFrames.Count > 0 && recordTimer - recordedFrames[0].timestamp > recordDuration)
            {
                recordedFrames.RemoveAt(0);
            }
        }

        private void SpawnClone()
        {
            activeClones.RemoveAll(c => c == null);

            if (activeClones.Count >= maxActiveClones)
            {
                Destroy(activeClones[0]);
                activeClones.RemoveAt(0);
            }

            if (clonePrefab == null)
            {
                Debug.LogWarning("ShadowClone: no clonePrefab assigned.");
                return;
            }

            GameObject clone = Instantiate(clonePrefab, recordedFrames[0].position, Quaternion.identity);
            var replay = clone.GetComponent<ShadowCloneReplay>();
            if (replay == null) replay = clone.AddComponent<ShadowCloneReplay>();

            var frames = new List<Vector3>();
            var facing = new List<bool>();
            foreach (var f in recordedFrames)
            {
                frames.Add(f.position);
                facing.Add(f.facingRight);
            }
            replay.Initialize(frames, facing, sampleInterval);

            activeClones.Add(clone);
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
        }
    }

    /// <summary>
    /// Attached automatically to spawned clone instances. Plays back a list of
    /// recorded positions over time so the clone retraces the player's path exactly.
    /// </summary>
    public class ShadowCloneReplay : MonoBehaviour
    {
        [SerializeField] private float lifetime = 10f;

        private List<Vector3> frames;
        private List<bool> facingFlags;
        private float interval;
        private float playbackTimer;
        private int currentIndex;

        public void Initialize(List<Vector3> recordedFrames, List<bool> facing, float sampleInterval)
        {
            frames = recordedFrames;
            facingFlags = facing;
            interval = sampleInterval;
            currentIndex = 0;
            playbackTimer = 0f;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (frames == null || frames.Count == 0) return;

            playbackTimer += Time.deltaTime;
            int targetIndex = Mathf.Clamp(Mathf.FloorToInt(playbackTimer / interval), 0, frames.Count - 1);

            transform.position = frames[targetIndex];

            if (facingFlags != null && targetIndex < facingFlags.Count)
            {
                Vector3 scale = transform.localScale;
                float sign = facingFlags[targetIndex] ? 1f : -1f;
                scale.x = Mathf.Abs(scale.x) * sign;
                transform.localScale = scale;
            }

            // Loop the replay so the clone keeps standing on switches etc.
            if (targetIndex >= frames.Count - 1)
            {
                playbackTimer = 0f;
            }
        }
    }
}
