using UnityEngine;
using UnityEngine.InputSystem;

public class ShopPurchaseTest : MonoBehaviour
{
    [SerializeField] private ShopItems testShopItem;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            testShopItem.Purchase();
        }
    }
}