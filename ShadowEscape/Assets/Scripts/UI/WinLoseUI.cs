using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.UI
{
    /// <summary>
    /// Hooks up Win screen (Next Level, Main Menu) and Lose screen
    /// (shown briefly before auto-respawn; can also offer a manual Retry button).
    /// Attach to: an empty GameObject inside the level's Canvas.
    /// </summary>
    public class WinLoseUI : MonoBehaviour
    {
        public void OnNextLevelClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.LoadNextLevel();
        }

        public void OnWinMainMenuClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.LoadMainMenu();
        }

        public void OnRetryClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.RestartLevel();
        }
    }
}
