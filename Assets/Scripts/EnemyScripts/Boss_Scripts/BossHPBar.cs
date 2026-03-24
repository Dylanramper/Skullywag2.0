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
        if(hpBar == true)
        hpBar.SetActive(false);
    }
    public void Show()
    {
        if(hpBar == false)
        hpBar.SetActive(true);
    }
}