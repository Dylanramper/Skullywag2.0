using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject upgradesShopPanel;
    [SerializeField] private GameObject itemsShopPanel;


    private void RefreshShopItems()
    {
        ShopItemUI[] shopItems = FindObjectsByType<ShopItemUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (ShopItemUI item in shopItems)
        {
            item.RefreshCurrentStat();
        }
    }


    public void OpenShopPanel()
    {
        shopPanel.SetActive(true);
        RefreshShopItems();
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
        RefreshShopItems();
    }

    public void OpenItemsPanel()
    {
        shopPanel.SetActive(false);
        itemsShopPanel.SetActive(true);
        RefreshShopItems();
    }

    public void BackToShop()
    {
        upgradesShopPanel.SetActive(false);
        itemsShopPanel.SetActive(false);
        shopPanel.SetActive(true);
        RefreshShopItems();
    }
}
