using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ShadowEscape.Player;

namespace ShadowEscape.Managers
{
    /// <summary>
    /// Central game state manager (singleton, persists across scenes).
    /// Handles pause state, player death/respawn flow, level completion,
    /// and delegates to SaveSystem for persistence.
    /// Attach to: an empty "GameManager" GameObject placed in the first-loaded scene only.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene Names")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string[] levelScenes = { "Level1", "Level2", "Level3" };

        [Header("Timing")]
        [SerializeField] private float respawnDelay = 1.5f;

        public bool IsPaused { get; private set; }
        public int CurrentLevelIndex { get; private set; }

        private PlayerHealth activePlayerHealth;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            IsPaused = false;
            Time.timeScale = 1f;

            for (int i = 0; i < levelScenes.Length; i++)
            {
                if (levelScenes[i] == scene.name) CurrentLevelIndex = i;
            }

            var player = FindObjectOfType<PlayerHealth>();
            if (player != null)
            {
                activePlayerHealth = player;
                activePlayerHealth.OnPlayerDied += HandlePlayerDied;

                // Restore saved checkpoint position for this level, if any
                SaveData data = SaveSystem.Load();
                if (data != null && data.HasCheckpointForLevel(scene.name))
                {
                    Vector3 pos = data.GetCheckpointForLevel(scene.name);
                    player.transform.position = pos;
                    player.SetCheckpoint(pos);
                }
            }
        }

        public void TogglePause()
        {
            SetPaused(!IsPaused);
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            UIManager.Instance?.ShowPauseMenu(paused);
        }

        private void HandlePlayerDied()
        {
            StartCoroutine(RespawnRoutine());
        }

        public void OnPlayerDeath()
        {
            UIManager.Instance?.ShowLoseScreen(true);
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            UIManager.Instance?.ShowLoseScreen(false);
            activePlayerHealth?.RespawnAtCheckpoint();
        }

        public void SaveCheckpoint(string checkpointId, Vector3 position)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SaveData data = SaveSystem.Load() ?? new SaveData();
            data.SetCheckpointForLevel(sceneName, position);
            data.highestLevelUnlocked = Mathf.Max(data.highestLevelUnlocked, CurrentLevelIndex);
            SaveSystem.Save(data);
        }

        public void CompleteLevel()
        {
            SaveData data = SaveSystem.Load() ?? new SaveData();
            data.highestLevelUnlocked = Mathf.Max(data.highestLevelUnlocked, CurrentLevelIndex + 1);
            SaveSystem.Save(data);

            UIManager.Instance?.ShowWinScreen(true);
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.victoryClip);
        }

        public void LoadNextLevel()
        {
            int next = CurrentLevelIndex + 1;
            if (next < levelScenes.Length)
            {
                SceneManager.LoadScene(levelScenes[next]);
            }
            else
            {
                SceneManager.LoadScene(mainMenuScene);
            }
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadLevelByIndex(int index)
        {
            if (index >= 0 && index < levelScenes.Length)
            {
                SceneManager.LoadScene(levelScenes[index]);
            }
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
