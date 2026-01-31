using UnityEngine;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject prefab;
        [Range(0f, 100f)]
        public float spawnWeight = 10f; // Higher weight = more likely to spawn
    }

    [Header("Spawn Settings")]
    public List<SpawnableItem> spawnableItems = new List<SpawnableItem>();

    [Header("Timing")]
    public float initialDelay = 5f; // Time before first spawn
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 20f;

    [Header("Spawn Area")]
    public bool spawnWithinCameraView = true;
    public Vector2 spawnAreaSize = new Vector2(8f, 4f); // If not using camera
    public float minDistanceFromPlayer = 3f;

    [Header("Limits")]
    public int maxActivePowerUps = 5;
    public bool respawnIfCollected = true;

    private float nextSpawnTime;
    private List<GameObject> activePowerUps = new List<GameObject>();
    private Transform player;
    private Camera mainCamera;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        mainCamera = Camera.main;

        // Set first spawn time
        nextSpawnTime = Time.time + initialDelay;

        Debug.Log("Power-up Spawner initialized. First spawn in " + initialDelay + " seconds.");
    }

    void Update()
    {
        // Clean up destroyed power-ups from the list
        activePowerUps.RemoveAll(item => item == null);

        // Check if it's time to spawn
        if (Time.time >= nextSpawnTime && activePowerUps.Count < maxActivePowerUps)
        {
            SpawnRandomPowerUp();

            // Set next spawn time
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnTime = Time.time + randomInterval;
        }
    }

    void SpawnRandomPowerUp()
    {
        if (spawnableItems.Count == 0)
        {
            Debug.LogWarning("No power-ups assigned to spawner!");
            return;
        }

        // Calculate total weight for weighted random selection
        float totalWeight = 0f;
        foreach (SpawnableItem item in spawnableItems)
        {
            if (item.prefab != null)
            {
                totalWeight += item.spawnWeight;
            }
        }

        if (totalWeight <= 0) return;

        // Pick a random value
        float randomPoint = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        GameObject selectedPrefab = null;

        foreach (SpawnableItem item in spawnableItems)
        {
            if (item.prefab == null) continue;

            currentWeight += item.spawnWeight;
            if (randomPoint <= currentWeight)
            {
                selectedPrefab = item.prefab;
                break;
            }
        }

        if (selectedPrefab == null) return;

        // Get spawn position
        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition == Vector3.zero) return; // No valid position found

        // Spawn the power-up
        GameObject newPowerUp = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        activePowerUps.Add(newPowerUp);

        Debug.Log("Spawned " + selectedPrefab.name + " at " + spawnPosition);
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        int attempts = 0;
        int maxAttempts = 20;

        while (attempts < maxAttempts)
        {
            if (spawnWithinCameraView && mainCamera != null)
            {
                // Get random position within camera view
                float randomX = Random.Range(-0.45f, 0.45f); // 90% of screen width
                float randomY = Random.Range(-0.45f, 0.45f); // 90% of screen height

                Vector3 viewportPos = new Vector3(randomX, randomY, 10f);
                spawnPosition = mainCamera.ViewportToWorldPoint(viewportPos);
                spawnPosition.z = 0; // 2D game
            }
            else
            {
                // Use defined spawn area
                float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

                spawnPosition = transform.position + new Vector3(randomX, randomY, 0);
            }

            // Check if position is too close to player
            if (player != null && Vector3.Distance(spawnPosition, player.position) < minDistanceFromPlayer)
            {
                attempts++;
                continue;
            }

            // Position is valid
            return spawnPosition;
        }

        Debug.LogWarning("Could not find valid spawn position after " + maxAttempts + " attempts.");
        return Vector3.zero;
    }

    // Public method to manually trigger a spawn
    public void SpawnNow()
    {
        SpawnRandomPowerUp();

        // Reset timer
        float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }

    // Method to spawn a specific power-up type
    public void SpawnSpecificPowerUp(GameObject powerUpPrefab)
    {
        if (powerUpPrefab == null || activePowerUps.Count >= maxActivePowerUps) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition == Vector3.zero) return;

        GameObject newPowerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        activePowerUps.Add(newPowerUp);
    }

    // Draw gizmos in editor to visualize spawn area
    void OnDrawGizmosSelected()
    {
        if (!spawnWithinCameraView)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0));
        }

        // Draw min distance from player
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }

    void OnGUI()
    {
        // Debug info on screen
        if (Time.time < 30f) // Show for first 30 seconds
        {
            float timeUntilNextSpawn = Mathf.Max(0, nextSpawnTime - Time.time);
            GUI.Label(new Rect(10, 200, 300, 20),
                $"Power-ups: {activePowerUps.Count}/{maxActivePowerUps}");
            GUI.Label(new Rect(10, 220, 300, 20),
                $"Next spawn in: {timeUntilNextSpawn:F1}s");
        }
    }
}