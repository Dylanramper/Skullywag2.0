using UnityEngine;
using System.Collections.Generic;

public class PowerUpWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public GameObject[] powerUpPrefabs; // Which power-ups to spawn
        public int spawnCount = 3;          // How many to spawn
        public float waveDelay = 10f;       // Wait time before wave
        public float spawnDelay = 1f;       // Time between each spawn
    }

    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>();

    [Header("Current Wave")]
    public int currentWaveIndex = 0;
    public bool autoStart = true;
    public bool loopWaves = true;

    [Header("Spawn Area")]
    public float spawnRadius = 5f;
    public Transform spawnCenter; // Leave empty for (0,0,0)

    void Start()
    {
        if (autoStart)
        {
            StartNextWave();
        }
    }

    public void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            if (loopWaves)
            {
                currentWaveIndex = 0; // Loop back to first wave
                Debug.Log("Looping waves...");
            }
            else
            {
                Debug.Log("All waves completed!");
                return;
            }
        }

        Wave currentWave = waves[currentWaveIndex];
        StartCoroutine(SpawnWave(currentWave));
    }

    System.Collections.IEnumerator SpawnWave(Wave wave)
    {

        // Wait before wave starts
        yield return new WaitForSeconds(wave.waveDelay);

        // Spawn each power-up in the wave
        for (int i = 0; i < wave.spawnCount; i++)
        {
            if (wave.powerUpPrefabs.Length == 0) break;

            // Pick random power-up from wave's list
            int randomIndex = Random.Range(0, wave.powerUpPrefabs.Length);
            GameObject powerUpPrefab = wave.powerUpPrefabs[randomIndex];

            if (powerUpPrefab != null)
            {
                SpawnSinglePowerUp(powerUpPrefab);
            }

            // Wait before next spawn
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        // Move to next wave
        currentWaveIndex++;
        StartNextWave();
    }

    void SpawnSinglePowerUp(GameObject prefab)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = spawnCenter != null ? spawnCenter.position : Vector3.zero;

        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomY = Random.Range(-spawnRadius, spawnRadius);

        return center + new Vector3(randomX, randomY, 0);
    }

    // For testing - shows spawn area in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = spawnCenter != null ? spawnCenter.position : Vector3.zero;
        Gizmos.DrawWireSphere(center, spawnRadius);
    }
}