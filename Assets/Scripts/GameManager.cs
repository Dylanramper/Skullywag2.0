using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Screen Shake")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float defaultShakeDuration;
    [SerializeField] private float defaultShakeMagnitude;

    private Vector3 originalCamPos;
    private Coroutine shakeCoroutine;

    [Header("UI Elements")]
    public Text scoreText;
    public Text highScoreText;

    private int currentScore = 0;
    private int highScore = 0;

    void Awake()
    {
        // Singleton pattern - only one GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keep between scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Load saved high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();

        if(cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        originalCamPos = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, GameObject.FindGameObjectWithTag("Player").transform.position.y, 0f);
    }

    public void AddScore(int points)
    {
        currentScore += points;

        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateUI();

        // Optional: Visual feedback
        Debug.Log($"Added {points} points! Total: {currentScore}");
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateUI();
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if(shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        float originalZ = cameraTransform.position.z;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(0f, 1f) * magnitude;
            float offsetY = Random.Range(0f, 1f) * magnitude;

            cameraTransform.localPosition = originalCamPos + new Vector3 (offsetX, offsetY, originalZ);

            elapsed += Time.deltaTime;
            yield return null; 
        }

        cameraTransform.position = new Vector3(originalCamPos.x, originalCamPos.y, originalZ);
    }
}