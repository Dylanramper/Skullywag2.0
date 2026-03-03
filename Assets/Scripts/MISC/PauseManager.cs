using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenuButtons;

    private bool isPaused;

    public void TogglePause()
    {
        if(isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        //Pause sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseBackground();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;

        //Resume Playing sound
        if(AudioManager.Instance != null)
            AudioManager.Instance.ResumeBackground();
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

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
