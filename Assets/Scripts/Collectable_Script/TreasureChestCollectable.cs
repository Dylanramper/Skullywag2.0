using UnityEngine;

public class TreasureChestCollectable : Collectable
{
    [Header("Treasure Chest Special")]
    public GameObject coinBurstEffect; // Optional: Spawn coins when collected
    public int coinBurstAmount = 5; // Number of mini coins to spawn

    protected override void Start()
    {
        // Override base values for chest
        coinValue = 10;
        rotationSpeed = 90f; // Slower rotation
        floatSpeed = 1.5f; // Slower float

        base.Start();
    }

    protected override void Collect()
    {
        // Do special chest effects before base collect
        if (coinBurstEffect != null)
        {
            for (int i = 0; i < coinBurstAmount; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(-0.5f, 0.5f),
                    0
                );
                Instantiate(coinBurstEffect, transform.position + randomOffset, Quaternion.identity);
            }
        }

        // Call base collect (adds score, plays sound, etc.)
        base.Collect();
    }
}