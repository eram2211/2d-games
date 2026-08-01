using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.UI
{
    /// <summary>
    /// Hooks up Pause Menu buttons: Resume, Restart, Main Menu.
    /// Also listens for the Escape key to toggle pause directly.
    /// Attach to: an empty GameObject inside the level's Canvas (sibling of UIManager).
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance?.TogglePause();
            }
        }

        public void OnResumeClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.SetPaused(false);
        }

        public void OnRestartClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.SetPaused(false);
            GameManager.Instance?.RestartLevel();
        }

        public void OnMainMenuClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.SetPaused(false);
            GameManager.Instance?.LoadMainMenu();
        }
    }
}
