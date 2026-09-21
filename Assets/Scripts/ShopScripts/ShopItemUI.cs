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

    [SerializeField] private TMP_Text currentDamageText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemNameText.text = shopItem.ItemName;
        itemDescriptionText.text = shopItem.Description;
        itemPriceText.text = shopItem.Price + " Coins";
        currentDamageText.text = "Current Damage: " + shopItem.GetCurrentDamage();

        buyButton.onClick.AddListener(BuyItem);        
    }

    private void BuyItem()
    {
        if(shopItem.Purchase())
        {
            currentDamageText.text = "Current Damage: " + shopItem.GetCurrentDamage();
        }
    }
}
