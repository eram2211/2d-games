using UnityEngine;
using UnityEngine.SceneManagement;
using ShadowEscape.Managers;

namespace ShadowEscape.UI
{
    /// <summary>
    /// Hooks up Main Menu buttons: New Game, Continue, Quit.
    /// Attach to: an empty GameObject in the MainMenu scene, wire buttons'
    /// OnClick() events to these public methods in the Inspector.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private string firstLevelScene = "Level1";
        [SerializeField] private GameObject continueButton;

        private void Start()
        {
            if (continueButton != null)
            {
                continueButton.SetActive(SaveSystem.HasSave());
            }
        }

        public void OnNewGameClicked()
        {
            SaveSystem.DeleteSave();
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            SceneManager.LoadScene(firstLevelScene);
        }

        public void OnContinueClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            SaveData data = SaveSystem.Load();
            int levelIndex = data != null ? data.highestLevelUnlocked : 0;
            GameManager.Instance?.LoadLevelByIndex(levelIndex);
        }

        public void OnQuitClicked()
        {
            AudioManager.Instance?.PlaySfx(AudioManager.Instance.buttonClickClip);
            GameManager.Instance?.QuitGame();
        }
    }
}
