using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [Header("Item Selector UI")]
    [SerializeField] private Button nextItemButton;
    [SerializeField] private Button itemButton;
    [SerializeField] private Button previousItemButton;

    [SerializeField] private Image itemButtonImage;
    [SerializeField] private TMP_Text itemCountText;

    [Header("Item Icons")]
    [SerializeField] private Sprite repairKitIcon;
    [SerializeField] private Sprite barrelIcon;
    [SerializeField] private Sprite shieldIcon;
    [SerializeField] private Sprite mortarIcon;
    [SerializeField] private Sprite chainShotIcon;
    [SerializeField] private Sprite homingRocketIcon;

    private int repairAmount = 20;

    private const string RepairKitKey = "RepairKitCount";
    private const string BarrelKey = "BarrelCount";
    private const string ShieldKey = "ShieldCount";
    private const string MortarKey = "MortarCount";
    private const string ChainShotKey = "ChainShotKey";
    private const string HomingRocketKey = "HomingRocketKey";

    [SerializeField] private int barrelCount = 0;
    [SerializeField] private int repairKitCount = 0;
    [SerializeField] private int shieldCount = 0;
    [SerializeField] private int mortarCount = 0;
    [SerializeField] private int chainShotCount = 0;
    [SerializeField] private int homingRocketCount = 0;

    private PlayerHealth playerHealth;
    private PlayerCannons playerCannons;
    private PlayerMortarController playerMortarController;

    private enum ItemType
    {
        RepairKit,
        Barrel,
        Shield,
        MortarShot,
        ChainShot,
        HomingRocket
    }

    private ItemType selectedItem = ItemType.RepairKit;

    private void Awake()
    {
        barrelCount = PlayerPrefs.GetInt(BarrelKey, 0);
        repairKitCount = PlayerPrefs.GetInt(RepairKitKey, 0);
        shieldCount = PlayerPrefs.GetInt(ShieldKey, 0);
        mortarCount = PlayerPrefs.GetInt(MortarKey, 0);
        chainShotCount = PlayerPrefs.GetInt(ChainShotKey, 0);
        homingRocketCount = PlayerPrefs.GetInt(HomingRocketKey, 0);

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerCannons = FindAnyObjectByType<PlayerCannons>();
        playerMortarController = FindAnyObjectByType<PlayerMortarController>();
    }

    private void Start()
    {

        if (nextItemButton != null)
        {
            nextItemButton.onClick.AddListener(SelectNextItem);
        }

        if (previousItemButton != null)
        {
            previousItemButton.onClick.AddListener(SelectPreviousItem);
        }

        if (itemButton != null)
        {
            itemButton.onClick.AddListener(UseSelectedItem);
        }

        UpdateSelectedItemUI();
    }

    private void SelectNextItem()
    {
        selectedItem++;

        if ((int)selectedItem >= System.Enum.GetValues(typeof(ItemType)).Length)
        {
            selectedItem = ItemType.RepairKit;
        }

        UpdateSelectedItemUI();
    }

    private void SelectPreviousItem()
    {
        selectedItem--;

        if ((int)selectedItem < 0)
        {
            selectedItem = (ItemType)(System.Enum.GetValues(typeof(ItemType)).Length - 1);
        }

        UpdateSelectedItemUI();
    }

    private void UpdateSelectedItemUI()
    {
        if (itemButtonImage != null)
        {
            switch (selectedItem)
            {
                case ItemType.RepairKit:
                    itemButtonImage.sprite = repairKitIcon;
                    break;

                case ItemType.Barrel:
                    itemButtonImage.sprite = barrelIcon;
                    break;

                case ItemType.Shield:
                    itemButtonImage.sprite = shieldIcon;
                    break;
                case ItemType.MortarShot:
                    itemButtonImage.sprite = mortarIcon;
                    break;
                case ItemType.ChainShot:
                    itemButtonImage.sprite = chainShotIcon;
                    break;
                case ItemType.HomingRocket:
                    itemButtonImage.sprite = homingRocketIcon;
                    break;  
            }
        }

        UpdateItemCountUI();
    }

    private void UpdateItemCountUI()
    {
        if (itemCountText == null)
            return;

        switch (selectedItem)
        {
            case ItemType.RepairKit:
                itemCountText.text = "x" + repairKitCount;
                break;

            case ItemType.Barrel:
                itemCountText.text = "x" + barrelCount;
                break;

            case ItemType.Shield:
                itemCountText.text = "x" + shieldCount;
                break;
            case ItemType.MortarShot:
                itemCountText.text = "x" + mortarCount;
                break;
            case ItemType.ChainShot:
                itemCountText.text = "x" + chainShotCount;
                break;
            case ItemType.HomingRocket:
                itemCountText.text = "x" + homingRocketCount;
                break;
        }
    }

    // =========================
    // USE SELECTED ITEM
    // =========================

    private void UseSelectedItem()
    {
        switch (selectedItem)
        {
            case ItemType.RepairKit:
                UseRepairKit();
                break;

            case ItemType.Barrel:
                UseBarrel();
                break;

            case ItemType.Shield:
                UseShield();
                break;
            case ItemType.MortarShot:
                UseMortarShot();
                break;
            case ItemType.ChainShot:
                UseChainShot();
                break;
            case ItemType.HomingRocket:
                UseHomingRocket();
                break;
        }
    }

    private void UseMortarShot()
    {
        Debug.Log("Mortar Shot button pressed.");

        if (mortarCount <= 0)
        {
            Debug.Log("No Mortar Shots available.");
            return;
        }

        if (playerMortarController == null)
        {
            Debug.LogError("PlayerMortarController not found.");
            return;
        }

        playerMortarController.BeginTargeting();
    }


    private void UseChainShot()
    {
        Debug.Log("Chain Shot button pressed.");

        if (chainShotCount <= 0)
        {
            Debug.Log("No Chain Shots available.");
            return;
        }

        if (playerMortarController == null)
        {
            Debug.LogError("PlayerMortarController not found.");
            return;
        }

        playerMortarController.BeginChainShotTargeting();
    }

    private void UseHomingRocket()
    {
        if (homingRocketCount <= 0)
        {
            Debug.Log("No Homing Rockets available.");
            return;
        }

        if (playerMortarController == null)
        {
            Debug.LogError("PlayerMortarController not found.");
            return;
        }

        playerMortarController.BeginHomingRocketTargeting();
    }

    private void UseBarrel()
    {
        if (barrelCount <= 0)
        {
            Debug.Log("No barrels available to use.");
            return;
        }

        if (playerCannons == null)
        {
            Debug.LogError("PlayerCannons not found.");
            return;
        }

        // Only remove the barrel if PlayerCannons successfully deployed it.
        if (!playerCannons.DeployBarrel())
        {
            Debug.Log("Barrel is still on cooldown.");
            return;
        }

        barrelCount--;

        PlayerPrefs.SetInt(BarrelKey, barrelCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Barrel used. Remaining: " + barrelCount);
    }

    private void UseShield()
    {
        if (shieldCount <= 0)
        {
            Debug.Log("No Shields available to use.");
            return;
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found.");
            return;
        }

        // Don't consume a shield if one is already active.
        if (playerHealth.hasShield)
        {
            Debug.Log("Shield is already active. Shield item not used.");
            return;
        }

        // Activate the existing shield system.
        playerHealth.ActivateShield(5f, 50);

        shieldCount--;

        PlayerPrefs.SetInt(ShieldKey, shieldCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Shield used. Remaining: " + shieldCount);
    }

    public bool ConsumeMortarShot()
    {
        if (mortarCount <= 0)
        {
            Debug.Log("No Mortar Shots available.");
            return false;
        }

        mortarCount--;

        PlayerPrefs.SetInt(MortarKey, mortarCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Mortar Shot used. Remaining: " + mortarCount);

        return true;
    }

    public void AddChainShot(int amount)
    {
        if (amount <= 0)
            return;

        chainShotCount += amount;

        PlayerPrefs.SetInt(ChainShotKey, chainShotCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Chain Shots added. Total: " + chainShotCount);
    }

    public bool ConsumeChainShot()
    {
        if (chainShotCount <= 0)
        {
            Debug.Log("No Chain Shots available.");
            return false;
        }

        chainShotCount--;

        PlayerPrefs.SetInt(ChainShotKey, chainShotCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Chain Shot used. Remaining: " + chainShotCount);
        return true;
    }

    public bool ConsumeHomingRocket()
    {
        if (homingRocketCount <= 0)
        {
            Debug.Log("No Homing Rockets available.");
            return false;
        }

        homingRocketCount--;

        PlayerPrefs.SetInt(HomingRocketKey, homingRocketCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Homing Rocket used. Remaining: " + homingRocketCount);
        return true;
    }

    // =========================
    // INVENTORY
    // =========================

    public int GetBarrelCount()
    {
        return barrelCount;
    }

    public int GetRepairKitCount()
    {
        return repairKitCount;
    }

    public int GetShieldCount()
    {
        return shieldCount;
    }

    public int GetMortarShotCount()
    {
        return mortarCount;
    }

    public int GetChainShotCount()
    {
        return chainShotCount;
    }

    public int GetHomingRocketCount()
    {
        return homingRocketCount;
    }

    public void AddMortarShot(int amount)
    {
        if (amount <= 0)
            return;

        mortarCount += amount;

        PlayerPrefs.SetInt(MortarKey, mortarCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Mortar Shots added: " + mortarCount);
    }

    public void AddBarrel(int amount)
    {
        if (amount <= 0)
            return;

        barrelCount += amount;

        PlayerPrefs.SetInt(BarrelKey, barrelCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Barrels added: " + barrelCount);
    }

    public void AddRepairKit(int amount)
    {
        if (amount <= 0)
            return;

        repairKitCount += amount;

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Repair Kits added: " + repairKitCount);
    }

    public void AddShield(int amount)
    {
        if (amount <= 0)
            return;

        shieldCount += amount;

        PlayerPrefs.SetInt(ShieldKey, shieldCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Shields added: " + shieldCount);
    }

    public void AddHomingRocket(int amount)
    {
        if (amount <= 0)
            return;

        homingRocketCount += amount;

        PlayerPrefs.SetInt(HomingRocketKey, homingRocketCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Homing Rockets added. Total: " + homingRocketCount);
    }

    public bool UseRepairKit()
    {
        if (repairKitCount <= 0)
        {
            Debug.Log("No Repair Kits available to use.");
            return false;
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth not found.");
            return false;
        }

        // Try to repair first.
        if (!playerHealth.Repair(repairAmount))
        {
            Debug.Log("Player health is already full. Repair kit not used.");
            return false;
        }

        // Only consume the Repair Kit if the repair was successful.
        repairKitCount--;

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Repair Kit used. Remaining: " + repairKitCount);

        return true;
    }
}
