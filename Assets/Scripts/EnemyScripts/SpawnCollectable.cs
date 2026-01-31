using UnityEngine;

public class SpawnCollectable : MonoBehaviour
{
    [Header("Collectable Drops")]
    public GameObject coinPrefab;
    public GameObject treasureChestPrefab;
    [Range(0f, 1f)]
    public float collectableDropChance = 0.6f; // 60% chance for collectable vs power-up

    void TrySpawnDrop()
    {
        // First decide: Power-up or Collectable?
        if (Random.value < collectableDropChance)
        {
            // Spawn collectable (coin or chest)
            SpawnCollectables();
        }
        else
        {
            // Spawn power-up (your existing power-up code)
            SpawnPowerUp();
        }
    }

    void SpawnCollectables()
    {
        GameObject collectableToDrop = null;
        float randomChoice = Random.value;

        // Make coins more common (80% chance) than chests (20% chance)
        if (randomChoice < 0.8f && coinPrefab != null)
        {
            collectableToDrop = coinPrefab;
            Debug.Log("Enemy dropped a Coin!");
        }
        else if (treasureChestPrefab != null)
        {
            collectableToDrop = treasureChestPrefab;
            Debug.Log("Enemy dropped a Treasure Chest!");
        }

        if (collectableToDrop != null)
        {
            Instantiate(collectableToDrop, transform.position, Quaternion.identity);
        }
    }

    // Your existing SpawnPowerUp method stays the same
    void SpawnPowerUp()
    {
        // Your existing power-up spawning code here
    }
}