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

    private ItemInventory itemInventory;

    void Start()
    {
        itemInventory = FindAnyObjectByType<ItemInventory>();
        itemNameText.text = shopItem.ItemName;
        itemDescriptionText.text = shopItem.Description;
        itemPriceText.text = shopItem.Price + " Coins";
        RefreshCurrentStat();

        buyButton.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        if (shopItem.Purchase())
        {
            RefreshCurrentStat();
        }
    }


    public void RefreshCurrentStat()
    {
        if (itemInventory == null)
        {
            itemInventory = FindAnyObjectByType<ItemInventory>();
        }

        if (currentStatText == null)
            return;

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

            case ShopItems.UpgradeType.RepairKit:
                currentStatText.text = "Repair Kits: " + itemInventory.GetRepairKitCount();
                break;

            case ShopItems.UpgradeType.ExplosiveBarrel:
                currentStatText.text = "Barrels: " + itemInventory.GetBarrelCount();
                break;

            case ShopItems.UpgradeType.Shield:
                currentStatText.text = "Shields: " + itemInventory.GetShieldCount();
                break;
            case ShopItems.UpgradeType.MortarShot:
                currentStatText.text = "Mortar Shots: " + itemInventory.GetMortarShotCount();
                break;
            case ShopItems.UpgradeType.ChainShot:
                currentStatText.text = "Chain Shots: " + itemInventory.GetChainShotCount();
                break;
            case ShopItems.UpgradeType.HomingRocket:
                currentStatText.text = "Homing Rockets: " + itemInventory.GetHomingRocketCount();
                break;
        }
    }

}
