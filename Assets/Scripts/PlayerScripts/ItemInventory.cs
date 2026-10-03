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

    [Header("Old Repair Kit UI - Temporary")]
    [SerializeField] private Button repairKitButton;
    [SerializeField] private TMP_Text repairKitCountText;

    private int repairAmount = 20;

    private const string RepairKitKey = "RepairKitCount";
    private const string BarrelKey = "BarrelCount";

    [SerializeField] private int barrelCount = 0;
    [SerializeField] private int repairKitCount = 0;

    private PlayerHealth playerHealth;
    private PlayerCannons playerCannons;

    private enum ItemType
    {
        RepairKit,
        Barrel
    }

    private ItemType selectedItem = ItemType.RepairKit;

    private void Awake()
    {
        barrelCount = PlayerPrefs.GetInt(BarrelKey, 0);
        repairKitCount = PlayerPrefs.GetInt(RepairKitKey, 0);

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerCannons = FindAnyObjectByType<PlayerCannons>();
    }

    private void Start()
    {
        // Keep the old repair kit button working temporarily.
        if (repairKitButton != null)
        {
            repairKitButton.onClick.AddListener(OnRepairKitButtonClicked);
        }

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

    private void OnRepairKitButtonClicked()
    {
        UseRepairKit();
    }

    // =========================
    // ITEM SELECTION
    // =========================

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
            selectedItem = ItemType.Barrel;
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
        }
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

        // Keep old repair kit UI updated while it still exists.
        if (repairKitCountText != null)
        {
            repairKitCountText.text = "x" + repairKitCount;
        }

        UpdateItemCountUI();

        Debug.Log("Repair Kits added: " + repairKitCount);
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

        if (repairKitCountText != null)
        {
            repairKitCountText.text = "x" + repairKitCount;
        }

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

        UpdateItemCountUI();

        Debug.Log("Repair Kit used. Remaining: " + repairKitCount);

        return true;
    }
}
