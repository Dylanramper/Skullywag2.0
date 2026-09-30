using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    private const string RepairKitKey = "RepairKitCount";

    [SerializeField] private int repairKitCount = 0;

    private void Awake()
    {
        repairKitCount = PlayerPrefs.GetInt(RepairKitKey, 0);
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

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

        Debug.Log("Repair Kits added: " + repairKitCount); 
    }

    public bool UseRepairKit()
    {
        if(repairKitCount <= 0)
        {
            Debug.Log("No Repair Kits available to use.");
            return false;
        }
        repairKitCount--;

        PlayerPrefs.SetInt(RepairKitKey, repairKitCount);
        PlayerPrefs.Save();

        Debug.Log("Repair Kit used. Remaining: " + repairKitCount);

        return true;
    }
}
