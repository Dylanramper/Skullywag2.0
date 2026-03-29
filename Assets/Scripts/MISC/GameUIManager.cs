using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject hud;
    private bool hasTapped = false;
    private Coroutine blinkRoutine;
    private EnemySpawner spawner;

    [SerializeField] private GameOverScroll mainMenuScroll;
    [SerializeField] private GameOverScroll pauseMenuScroll;
    [SerializeField] private TextMeshProUGUI continueTXT;

    bool IsTap()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    void Start()
    {
        Time.timeScale = 0f;
        spawner = FindFirstObjectByType<EnemySpawner>();
        spawner.enabled = false;

        continueTXT.gameObject.SetActive(true);
        blinkRoutine = StartCoroutine(BlinkLoop());
        hud.SetActive(false);
    }

    void Update()
    {
        if (hasTapped) return;

        if (IsTap())
        {
            hasTapped = true;
            OnFirstTap();
        }
    }

    void OnFirstTap()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        continueTXT.gameObject.SetActive(false);

        ShowMainMenu();
    }

    public void ShowMainMenu()
    {

        Time.timeScale = 0f;
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        hud.SetActive(false);

        AudioManager.Instance.PlayMusic(menuMusic);

        mainMenuScroll.ShowScroll();
    }

    public void StartGame()
    {
        AudioManager.Instance.PlayMusic(gameplayMusic);

        mainMenuScroll.CloseMenu(() => {
            Time.timeScale = 1f;

            hud.SetActive(true);
            spawner.enabled = true; });
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);
        pauseMenuScroll.ShowScroll();
    }

    public void ResumeGame()
    {
        pauseMenuScroll.CloseMenu(OnResumeComplete);
    }

    private void OnResumeComplete()
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
        AudioManager.Instance.PlayMusic(gameplayMusic);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            continueTXT.enabled = !continueTXT.enabled;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}