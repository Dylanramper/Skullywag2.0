using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image fillImage;
    public GameObject hpBar;

    public void UpdateHealth(float current, float max)
    {
        if(max <= 0)
            return;

        fillImage.fillAmount = Mathf.Clamp01(current / max);
    }
    public void Hide()
    {
        hpBar.SetActive(false);
    }
    public void Show()
    {
        hpBar.SetActive(true);
    }
}