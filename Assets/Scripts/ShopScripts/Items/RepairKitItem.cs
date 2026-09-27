using UnityEngine;
using UnityEngine.Rendering;

public class RepairKitItem : MonoBehaviour
{
    [SerializeField] private int repairAmount = 20;

    private PlayerHealth playerHealth;
    private ItemInventory itemInventory;

    private void Awake()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        itemInventory = FindAnyObjectByType<ItemInventory>();

    }

    public void UseRepairKit()
    {
        if(playerHealth == null || itemInventory == null)
        {
            Debug.LogError("PlayerHealth or ItemInventory not found.");
            return;
        }

        //Try repair first
        if(!playerHealth.Repair(repairAmount))
        {
            Debug.Log("Player health is already full. Repair kit not used.");
            return;
        }
        //If repair was successful, use the repair kit
        itemInventory.UseRepairKit();
    }
}
