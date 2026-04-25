using System.Collections.Generic;
using UnityEngine;

public class ReticleSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject reticlePrefab;

    private Transform player;

    [Header("Spawn Settings")]
    public int reticleCount = 6;
    public float spawnRadius = 4f;

    [Header("Timing")]
    public float reticleLifetime = 1f;

    public List<Vector2> reticlePositions = new List<Vector2>();

    void Awake()
    {
        // Try to find player automatically
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found! Make sure it has the 'Player' tag.");
    }

    //Temp (Delete this method after) 
    void Start()
    {
        Invoke(nameof(SpawnReticles), 1f);
    }

    public void SpawnReticles()
    {
        if (player == null) return;

        reticlePositions.Clear();

        Vector2 basePosition = player.position;

        for (int i = 0; i < reticleCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPos = basePosition + offset;

            reticlePositions.Add(spawnPos);

            GameObject reticle = Instantiate(reticlePrefab, spawnPos, Quaternion.identity);
            Destroy(reticle, reticleLifetime);
        }
    }
}