using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopItems : MonoBehaviour
{
    [Header("Shop Item")]
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private int price = 100;


    public string ItemName => itemName;
    public string Description => description;
    public int Price => price;

    public bool Purchase()
    {
        if(CurrencyManager.Instance == null)
        {
            Debug.Log("CurrencyManager not found");
            return false;
        }

        if (!CurrencyManager.Instance.SpendCoins(price))
        {
            Debug.Log("Not Enough coins for " + itemName);
            return false;
        }

        ApplyUpgrade();

        Debug.Log("Purchased " + itemName);

        return true;
    }

    private void ApplyUpgrade()
    {
        //Add upgrades here----------------------------------------------------------------------------------
    }
}
