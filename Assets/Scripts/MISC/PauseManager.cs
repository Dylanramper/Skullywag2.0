using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenuButtons;
    [SerializeField] private GameObject gameStartCountdown;

    [SerializeField] private GameOverScroll pauseMenuScroll;

    private bool isPaused;
    public bool textCountdownActive;

    public void TogglePause()
    {
        if(isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenuScroll.ShowScroll(); // reuse animation
    }

    public void ResumeGame()
    {
        pauseMenuScroll.CloseMenu(() => {  });
    }

    public void OpenSettings()
    {
        pauseMenuButtons.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackFromSettings()
    {
        pauseMenuButtons.SetActive(true);
        settingsMenu.SetActive(false);
    }
}
