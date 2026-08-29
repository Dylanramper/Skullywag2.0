using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private ShopItems ShopItems;
    [SerializeField] private Button buyButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buyButton.onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        ShopItems.Purchase();
    }
}
