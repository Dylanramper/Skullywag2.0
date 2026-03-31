using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject pauseMenuButtons;
    [SerializeField] private GameObject gameStartCountdown;

    [SerializeField] private GameOverScroll pauseMenuScroll;

    public bool isMainActive = true;
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

        pauseMenuScroll.ShowScroll();
    }

    public void ResumeGame()
    {
        pauseMenuScroll.CloseMenu(() => {  });
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        mainMenuButtons.SetActive(false);
        pauseMenuButtons.SetActive(false);
    }

    public void BackFromSettings()
    {
        if (isMainActive)
        {
            mainMenuButtons.SetActive(true);
            settingsMenu.SetActive(false);
        }
        else
        {
            pauseMenuButtons.SetActive(true);
            settingsMenu.SetActive(false);
        } 
    }
}
