using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [SerializeField] private Button repairKitButton;
    [SerializeField] private TMP_Text repairKitCountText;
    private int repairAmount = 20;

    private const string RepairKitKey = "RepairKitCount";

    [SerializeField] private int repairKitCount = 0;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        repairKitCount = PlayerPrefs.GetInt(RepairKitKey, 0);
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    private void Start()
    {
        repairKitCountText.text = "x" + repairKitCount;
        repairKitButton.onClick.AddListener(OnRepairKitButtonClicked);
    }

    private void OnRepairKitButtonClicked()
    {
        UseRepairKit();
    }

    public int GetRepairKitCount()
    {
        return repairKitCount;
    }

    public void AddRepairKit(int amount)
    {
        if (amount <= 0)
            return;

        repairKitCount += amount;
        repairKitCountText.text = "x" + repairKitCount;

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

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

        // Try to repair first
        if (!playerHealth.Repair(repairAmount))
        {
            Debug.Log("Player health is already full. Repair kit not used.");
            return false;
        }

        // Only consume the Repair Kit if the repair was successful
        repairKitCount--;
        repairKitCountText.text = "x" + repairKitCount;

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

        Debug.Log("Repair Kit used. Remaining: " + repairKitCount);

        return true;
    }
}
