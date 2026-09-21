using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;

    public void OpenShopPanel()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
