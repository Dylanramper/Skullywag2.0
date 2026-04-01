using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton so other scripts can easily access it
    public static ScoreManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI scoreText;         // Drag your UI Text here

    private int currentScore = 0;

    void Awake()
    {
        // Make sure only one ScoreManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    // Call this method to add points
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreText();
        Debug.Log($"Score: {currentScore} (+{points})");
    }

    // Update the UI text
    void UpdateScoreText()
    {
        //if (scoreText != null)
            //scoreText.text = "Score: " + currentScore;
    }

    // Optional: get current score
    public int GetScore()
    {
        return currentScore;
    }

    // Optional: reset score
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }
}