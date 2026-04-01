using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    public GameObject loadingScreen;
    public Slider progressBar; // optional
    public TMPro.TextMeshProUGUI progressText; // optional

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // progress is 0..0.9 while loading, 0.9..1 when activated
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null) progressBar.value = progress;
            if (progressText != null) progressText.text = $"{(int)(progress * 100)}%";

            // Activate scene when fully loaded
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}