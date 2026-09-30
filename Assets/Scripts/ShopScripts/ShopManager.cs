using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject upgradesShopPanel;
    [SerializeField] private GameObject itemsShopPanel;

    public void OpenShopPanel()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
        upgradesShopPanel.SetActive(false);
        itemsShopPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenUpgradesPanel()
    {
        shopPanel.SetActive(false);
        upgradesShopPanel.SetActive(true);
    }

    public void OpenItemsPanel()
    {
        shopPanel.SetActive(false);
        itemsShopPanel.SetActive(true);
    }

    public void BackToShop()
    {
        upgradesShopPanel.SetActive(false);
        itemsShopPanel.SetActive(false);
        shopPanel.SetActive(true);
    }
}
