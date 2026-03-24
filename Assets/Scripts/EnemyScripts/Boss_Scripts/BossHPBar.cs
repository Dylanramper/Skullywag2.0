using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image fillImage;
    private float fillSmoothSpeed = 5f;

    public void UpdateHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
        //fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, (float)current / max, fillSmoothSpeed * Time.deltaTime);
    }
}
