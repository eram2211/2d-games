using UnityEngine;

namespace ShadowEscape.Managers
{
    /// <summary>
    /// Central UI singleton that shows/hides the pause, win and lose panels.
    /// Attach to: a "UIManager" GameObject inside each gameplay level's Canvas
    /// (one per level scene, since UI panels are scene-specific).
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject hudPanel;

        private void Awake()
        {
            Instance = this;
            SetActiveSafe(pausePanel, false);
            SetActiveSafe(winPanel, false);
            SetActiveSafe(losePanel, false);
            SetActiveSafe(hudPanel, true);
        }

        public void ShowPauseMenu(bool show) => SetActiveSafe(pausePanel, show);
        public void ShowWinScreen(bool show) => SetActiveSafe(winPanel, show);
        public void ShowLoseScreen(bool show) => SetActiveSafe(losePanel, show);

        private void SetActiveSafe(GameObject go, bool active)
        {
            if (go != null) go.SetActive(active);
        }
    }
}
