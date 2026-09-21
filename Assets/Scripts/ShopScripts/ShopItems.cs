using UnityEngine;

public class ShopItems : MonoBehaviour
{
    public enum UpgradeType
    {
        CannonDamage,
        MaxHealth,
        MaxSpeed,
        BarrelDamage
    }

    [Header("Shop Item")]
    [SerializeField] private string itemName;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private int price = 100;
    [SerializeField] private UpgradeType upgradeType;
    private PlayerCannons playerCannons;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    public string ItemName => itemName;
    public string Description => description;
    public int Price => price;
    public UpgradeType Type => upgradeType;

    public int GetCurrentDamage()
    {
        if(playerCannons == null)
        {
            return 0;
        }
        
        return playerCannons.GetCurrentDamage();
    }

    public int GetCurrentHealth()
    {
        if (playerHealth == null)
        {
            return 0;
        }

        return playerHealth.maxHealth;
    }

    public float GetCurrentSpeed()
    {
        if(playerMovement == null)
        {
            return 0;
        }

        return playerMovement.speed;
    }

    public int GetCurrentBarrelDamage()
    {
        return PlayerPrefs.GetInt("BarrelDamageKey", 30);
    }

    private void Awake()
    {
        playerCannons = FindAnyObjectByType<PlayerCannons>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
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
            case UpgradeType.MaxHealth:
                if(playerHealth != null)
                {
                    playerHealth.IncreaseMaxHealth(20);
                }
                break;
            case UpgradeType.MaxSpeed:
                if(playerMovement != null)
                {
                    playerMovement.IncreasePermanentSpeed(0.5f);
                }
                break;
            case UpgradeType.BarrelDamage:
                ExplosiveBarrel.IncreasePermanentBarrelDamage(15);
                break;
        }
    }
}
