using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int rowboats;
        public int brigs;
        public int galleons;
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private GameStartCountdown countdownManager;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    private int totalEnemiesThisWave;

    //------------------------------------Edit
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject rowboatPrefab;
    [SerializeField] private GameObject brigPrefab;
    [SerializeField] private GameObject galleonPrefab;

    [Header("Spawn Timing")]
    public float initialDelay = 5f;

    [Header("Spawn Area")]
    public bool spawnWithinCameraView = false;
    public Vector2 spawnAreaSize = new Vector2(10f, 6f); // If not using camera
    public float minDistanceFromPlayer = 10f; // Don't spawn too close

    [Header("Limits")]
    public int maxEnemiesOnScreen = 10;

    [Header("Enemy Indicators")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Transform indicatorParent;

    // Internal variables
    private Transform player;
    private Camera mainCamera;
    private List<GameObject> activeEnemies = new List<GameObject>();

    //------------------------------------Edit
    private int currentWave = 0;
    private bool waveInProgress = false;

    void Start()
    {
        /*
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        mainCamera = Camera.main;

        // Set first spawn time
        nextSpawnTime = Time.time + initialDelay;

        Debug.Log("Enemy Spawner initialized. First spawn in " + initialDelay + " seconds.");
        */

        //------------------------------------Edit
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        mainCamera = Camera.main;

        Invoke(nameof(StartNextWave), initialDelay);
    }

    void Update()
    {
        /*
        // Clean up destroyed enemies from the list
        activeEnemies.RemoveAll(item => item == null);

        // Check if it's time to spawn and we haven't reached the limit
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemiesOnScreen)
        {
            SpawnEnemy();

            // Set next spawn time
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnTime = Time.time + randomInterval;
        }*/

        //------------------------------------Edit
        activeEnemies.RemoveAll(item => item == null);
        UpdateEnemyUI();

        if(waveInProgress && activeEnemies.Count == 0)
        {
            waveInProgress = false;
            StartCoroutine(StartNextWaveRoutine());
        }
    }

    IEnumerator StartNextWaveRoutine()
    {
        yield return StartCoroutine(countdownManager.WaveCountdown());
        StartNextWave();

        Wave wave = waves[currentWave - 1];
    }

    void UpdateEnemyUI()
    {
        if (enemyCountText == null) return;

        int alive = activeEnemies.Count;
        enemyCountText.text = alive + " / " + totalEnemiesThisWave;

        enemyCountText.gameObject.SetActive(waveInProgress);
    }

    void StartNextWave()
    {
        currentWave++;

        if(currentWave > waves.Count)
        {
            StartBossWave();
            return;
        }

        waveInProgress = true;

        Wave wave = waves[currentWave - 1];

        totalEnemiesThisWave = wave.rowboats + wave.brigs + wave.galleons;

        UpdateEnemyUI();

        SpawnWave(wave.rowboats, wave.brigs, wave.galleons);
    }

    void SpawnWave(int rowboats, int brigs, int galleons)
    {
        StartCoroutine(SpawnWaveRoutine(rowboats, brigs, galleons));
    }

    IEnumerator SpawnWaveRoutine(int rowboats, int brigs, int galleons)
    {
        for(int i = 0; i < rowboats; i++)
        {
            SpawnEnemy(rowboatPrefab);
            yield return new WaitForSeconds(1f);
        }

        for (int i = 0; i < brigs; i++)
        {
            SpawnEnemy(brigPrefab);
            yield return new WaitForSeconds(1.5f);
        }
        for(int i = 0;i < galleons; i++)
        {
            SpawnEnemy(galleonPrefab);
            yield return new WaitForSeconds(2f);
        }
    }

    void SpawnEnemy(GameObject prefab)
    {
        //-------------------------------------Edit
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 spawnPosition = GetRandomSpawnPosition(playerPos);

        GameObject newEnemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);
        UpdateEnemyUI();

        if(indicatorPrefab != null && indicatorParent != null)
        {
            GameObject indicator = Instantiate(indicatorPrefab, indicatorParent);
            indicator.GetComponent<EnemyIndicator>().Initialize(newEnemy.transform);
        }
    }

    void StartBossWave()
    {
        Debug.Log("Boss Wave!!");
    }

    Vector3 GetRandomSpawnPosition(Vector3 playerPos)
    {
        Vector3 spawnPosition = Vector3.zero;
        int attempts = 0;
        int maxAttempts = 20;

        while (attempts < maxAttempts)
        {
            if (!spawnWithinCameraView && mainCamera != null)
            {
                float spawnRadius = Random.Range(10f, 18f); //Distance from player
                float angle = Random.Range(0f, Mathf.PI * 2);

                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spawnRadius;

                spawnPosition = playerPos + offset;
            }
            else
            {
                // Use defined area
                float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
                spawnPosition = transform.position + new Vector3(randomX, randomY, 0);
            }

            // Check distance from player
            if (Vector3.Distance(spawnPosition, playerPos) < minDistanceFromPlayer)
            {
                attempts++;
                continue;
            }

            // Valid position found
            return spawnPosition;
        }

        Debug.LogWarning("Could not find valid spawn position after " + maxAttempts + " attempts.");
        return spawnPosition;
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