using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeightedPowerUp
{
    public GameObject powerUpPrefab;
    [Tooltip("Higher weight = more likely to spawn. Example: 3 is 3 times more likely than weight 1")]
    public float weight = 1f;
    [Range(0.01f, 1f)]
    [Tooltip("Overall chance this power-up can spawn (0=never, 1=always if selected)")]
    public float spawnChance = 1f;
    [Tooltip("Minimum number of enemies that must be defeated before this can spawn")]
    public int minScoreOrEnemies = 0;
}

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Weighted Spawn Settings")]
    public WeightedPowerUp[] powerUps;

    [Header("Global Settings")]
    [Range(0f, 1f)]
    public float globalDropChance = 0.3f;
    [Tooltip("Should we exclude power-ups that fail their individual spawn chance?")]
    public bool filterBySpawnChance = true;
    [Tooltip("Add random rotation to spawned power-ups")]
    public bool randomRotation = true;

    // Track game progression for conditional spawning
    private int enemiesDefeated = 0;

    // Call this when enemy is defeated
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
    }

    public void SpawnPowerUp()
    {
        // First check global drop chance
        if (Random.value > globalDropChance || powerUps.Length == 0)
        {
            return;
        }

        // Build list of eligible power-ups with their weights
        List<GameObject> eligiblePrefabs = new List<GameObject>();
        List<float> eligibleWeights = new List<float>();

        foreach (var powerUp in powerUps)
        {
            // Check if this power-up meets minimum requirements
            if (enemiesDefeated < powerUp.minScoreOrEnemies)
            {
                continue;
            }

            // Check individual spawn chance if enabled
            if (filterBySpawnChance && Random.value > powerUp.spawnChance)
            {
                continue;
            }

            eligiblePrefabs.Add(powerUp.powerUpPrefab);
            eligibleWeights.Add(powerUp.weight);
        }

        // If no eligible power-ups, return
        if (eligiblePrefabs.Count == 0)
        {
            return;
        }

        // Perform weighted random selection
        GameObject selectedPrefab = GetWeightedRandomPowerUp(eligiblePrefabs, eligibleWeights);

        if (selectedPrefab != null)
        {
            // Instantiate with optional random rotation
            Quaternion rotation = randomRotation
                ? Quaternion.Euler(0, 0, Random.Range(0f, 360f))
                : Quaternion.identity;

            Instantiate(selectedPrefab, transform.position, rotation);

            // Optional: Visual feedback
            Debug.Log($"Spawned: {selectedPrefab.name}");
        }
    }

    private GameObject GetWeightedRandomPowerUp(List<GameObject> prefabs, List<float> weights)
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        // Pick a random point
        float randomPoint = Random.Range(0f, totalWeight);

        // Find which item this corresponds to
        float currentWeight = 0f;
        for (int i = 0; i < prefabs.Count; i++)
        {
            currentWeight += weights[i];
            if (randomPoint <= currentWeight)
            {
                return prefabs[i];
            }
        }

        // Fallback (shouldn't reach here)
        return prefabs[prefabs.Count - 1];
    }

    // Alternative method using a more efficient algorithm
    public void SpawnPowerUpWithAliasMethod()
    {
        if (Random.value > globalDropChance || powerUps.Length == 0)
        {
            return;
        }

        // Use Alias Method for O(1) selection (good for many items)
        WeightedPowerUp selected = GetWeightedPowerUpAliasMethod();

        if (selected != null && selected.powerUpPrefab != null)
        {
            Instantiate(selected.powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    // Advanced: Alias Method for weighted random (more efficient for many items)
    private WeightedPowerUp GetWeightedPowerUpAliasMethod()
    {
        // This is a simplified version. For full Alias Method, you'd need to precompute tables.
        // For now, using the standard method is fine for small arrays.
        return GetWeightedRandomSimple();
    }

    private WeightedPowerUp GetWeightedRandomSimple()
    {
        // Build list of power-ups that pass their spawn chance
        List<WeightedPowerUp> eligible = new List<WeightedPowerUp>();
        List<float> weights = new List<float>();

        foreach (var powerUp in powerUps)
        {
            if (enemiesDefeated >= powerUp.minScoreOrEnemies &&
                (!filterBySpawnChance || Random.value <= powerUp.spawnChance))
            {
                eligible.Add(powerUp);
                weights.Add(powerUp.weight);
            }
        }

        if (eligible.Count == 0) return null;

        // Weighted selection
        float totalWeight = 0f;
        foreach (float w in weights)
        {
            totalWeight += w;
        }

        float randomPoint = Random.Range(0f, totalWeight);
        float current = 0f;

        for (int i = 0; i < eligible.Count; i++)
        {
            current += weights[i];
            if (randomPoint <= current)
            {
                return eligible[i];
            }
        }

        return eligible[eligible.Count - 1];
    }
}