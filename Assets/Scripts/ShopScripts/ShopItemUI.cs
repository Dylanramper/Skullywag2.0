using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private ShopItems shopItem;
    [SerializeField] private Button buyButton;

    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemPriceText;

    [SerializeField] private TMP_Text currentStatText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemNameText.text = shopItem.ItemName;
        itemDescriptionText.text = shopItem.Description;
        itemPriceText.text = shopItem.Price + " Coins";
        UpdateCurrentStat();

        buyButton.onClick.AddListener(BuyItem);        
    }

    private void BuyItem()
    {
        if(shopItem.Purchase())
        {
            UpdateCurrentStat();
        }
    }

    private void UpdateCurrentStat()
    {
        switch (shopItem.Type)
        {
            case ShopItems.UpgradeType.CannonDamage:
                currentStatText.text = "Current Damage: " + shopItem.GetCurrentDamage();
                break;
            case ShopItems.UpgradeType.MaxHealth:
                currentStatText.text = "Current Health: " + shopItem.GetCurrentHealth();
                break;
            case ShopItems.UpgradeType.MaxSpeed:
                currentStatText.text = "Current Speed: " + shopItem.GetCurrentSpeed();
                break;
            case ShopItems.UpgradeType.BarrelDamage:
                currentStatText.text = "Current Barrel Damage: " + shopItem.GetCurrentBarrelDamage();
                break;
        }
    }
}
