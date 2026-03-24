using UnityEngine;
using UnityEngine.UI;

public class PowerUpTimerUI : MonoBehaviour
{
    public Image fillImage;

    private float duration;
    private float timer;
    private bool isActive = false;

    public void StartTimer(float powerUpDuration)
    {
        duration = powerUpDuration;
        timer = powerUpDuration;
        isActive = true;

        gameObject.SetActive(true);
        fillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;

        fillImage.fillAmount = timer / duration;

        if (timer <= 0f)
        {
            isActive = false;
            gameObject.SetActive(false);
        }
    }
}