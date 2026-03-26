using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject hud;

    void Start()
    {
        ShowMainMenu();
        FindFirstObjectByType<EnemySpawner>().enabled = false;
    }

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;

        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        hud.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        mainMenu.SetActive(false);
        hud.SetActive(true);
        FindFirstObjectByType<EnemySpawner>().enabled = true;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        pauseMenu.SetActive(false);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;

        gameOverMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}