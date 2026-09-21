using UnityEngine;

public class ShopItems : MonoBehaviour
{
    public enum UpgradeType
    {
        CannonDamage
    }

    [Header("Shop Item")]
    [SerializeField] private string itemName;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private int price = 100;
    [SerializeField] private UpgradeType upgradeType;
    private PlayerCannons playerCannons;

    public string ItemName => itemName;
    public string Description => description;
    public int Price => price;

    public int GetCurrentDamage()
    {
        if(playerCannons == null)
        {
            return 0;
        }
        
        return playerCannons.GetCurrentDamage();
    }

    private void Awake()
    {
        playerCannons = FindAnyObjectByType<PlayerCannons>();
    }

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
        switch (upgradeType)
        {
            case UpgradeType.CannonDamage:
                if(playerCannons != null)
                {
                    playerCannons.IncreasePermanentDamage(5);
                }
                break;
        }
    }
}
