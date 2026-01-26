using UnityEngine;
using System.Collections.Generic;

public class GamePowerUpManager : MonoBehaviour
{
    public static GamePowerUpManager Instance;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float baseDropChance = 0.3f;
    public float dropChanceIncreasePerEnemy = 0.005f;
    public float maxDropChance = 0.8f;

    private int totalEnemiesKilled = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this from your enemy's Die() method
    public void ReportEnemyDeath(GameObject enemy, PowerUpSpawner spawner)
    {
        totalEnemiesKilled++;

        if (spawner != null)
        {
            // Calculate current drop chance
            float currentDropChance = Mathf.Min(
                baseDropChance + (totalEnemiesKilled * dropChanceIncreasePerEnemy),
                maxDropChance
            );

            // Roll for drop
            if (Random.value <= currentDropChance)
            {
                spawner.SpawnPowerUp();
            }
        }
    }
}