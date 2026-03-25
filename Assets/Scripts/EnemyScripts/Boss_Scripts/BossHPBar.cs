using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image fillImage;
    public GameObject hpBar;

    public void UpdateHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
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