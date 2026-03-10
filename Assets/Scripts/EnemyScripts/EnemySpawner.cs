using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        [Range(0f, 100f)]
        public float spawnWeight = 10f; // Higher = more common
    }

    [Header("Enemy Types")]
    public List<EnemyType> enemyTypes = new List<EnemyType>();

    [Header("Spawn Timing")]
    public float initialDelay = 5f;        // Time before first spawn
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 8f;

    [Header("Spawn Area")]
    public bool spawnWithinCameraView = false;
    public Vector2 spawnAreaSize = new Vector2(10f, 6f); // If not using camera
    public float minDistanceFromPlayer = 10f; // Don't spawn too close

    [Header("Limits")]
    public int maxEnemiesOnScreen = 10;

    // Internal variables
    private float nextSpawnTime;
    private Transform player;
    private Camera mainCamera;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        mainCamera = Camera.main;

        // Set first spawn time
        nextSpawnTime = Time.time + initialDelay;

        Debug.Log("Enemy Spawner initialized. First spawn in " + initialDelay + " seconds.");
    }

    void Update()
    {
        // Clean up destroyed enemies from the list
        activeEnemies.RemoveAll(item => item == null);

        // Check if it's time to spawn and we haven't reached the limit
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemiesOnScreen)
        {
            SpawnEnemy();

            // Set next spawn time
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnTime = Time.time + randomInterval;
        }
    }

    void SpawnEnemy()
    {
        if (enemyTypes.Count == 0)
        {
            Debug.LogWarning("No enemy types assigned to spawner!");
            return;
        }

        // Calculate total weight
        float totalWeight = 0f;
        foreach (EnemyType type in enemyTypes)
        {
            if (type.enemyPrefab != null)
                totalWeight += type.spawnWeight;
        }

        if (totalWeight <= 0) return;

        // Pick a random enemy type based on weight
        float randomPoint = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        GameObject selectedPrefab = null;

        foreach (EnemyType type in enemyTypes)
        {
            if (type.enemyPrefab == null) continue;

            currentWeight += type.spawnWeight;
            if (randomPoint <= currentWeight)
            {
                selectedPrefab = type.enemyPrefab;
                break;
            }
        }

        if (selectedPrefab == null) return;

        // Get spawn position
        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition == Vector3.zero) return; // No valid position

        // Spawn enemy
        GameObject newEnemy = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        Debug.Log("Spawned " + selectedPrefab.name + " at " + spawnPosition);
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        int attempts = 0;
        int maxAttempts = 20;

        while (attempts < maxAttempts)
        {
            if (!spawnWithinCameraView && mainCamera != null)
            {
                // Random position within camera view (but not at edges)
                float randomX = Random.Range(-0.45f, 0.45f); // 90% of screen width
                float randomY = Random.Range(-0.45f, 0.45f); // 90% of screen height

                Vector3 viewportPos = new Vector3(randomX, randomY, 10f);
                spawnPosition = mainCamera.ViewportToWorldPoint(viewportPos);
                spawnPosition.z = 0; // For 2D
            }
            else
            {
                // Use defined area
                float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
                spawnPosition = transform.position + new Vector3(randomX, randomY, 0);
            }

            // Check distance from player
            if (player != null && Vector3.Distance(spawnPosition, player.position) < minDistanceFromPlayer)
            {
                attempts++;
                continue;
            }

            // Also check if position is inside the camera view (if using camera)
            if (!spawnWithinCameraView && mainCamera != null)
            {
                Vector3 viewportCheck = mainCamera.WorldToViewportPoint(spawnPosition);
                if (viewportCheck.x < 0 || viewportCheck.x > 1 || viewportCheck.y < 0 || viewportCheck.y > 1)
                {
                    attempts++;
                    continue;
                }
            }

            // Valid position found
            return spawnPosition;
        }

        Debug.LogWarning("Could not find valid spawn position after " + maxAttempts + " attempts.");
        return Vector3.zero;
    }

    // Visualize spawn area in editor
    void OnDrawGizmosSelected()
    {
        if (!spawnWithinCameraView)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0));
        }

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}