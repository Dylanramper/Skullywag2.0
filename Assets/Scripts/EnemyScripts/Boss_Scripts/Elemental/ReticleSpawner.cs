using System.Collections.Generic;
using UnityEngine;

public class ReticleSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject reticlePrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public int reticleCount = 6;
    public float spawnRadius = 4f;

    [Header("Timing")]
    public float reticleLifetime = 1.0f;

    // Store positions for ScatterProjectile later
    public List<Vector2> reticlePositions = new List<Vector2>();

    public void SpawnReticles()
    {
        reticlePositions.Clear();

        Vector2 basePosition = player.position;

        for (int i = 0; i < reticleCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPos = basePosition + offset;
            
            reticlePositions.Add(spawnPos);

            GameObject reticle = Instantiate(reticlePrefab, spawnPos, Quaternion.identity);

            // Auto destroy (visual only)
            Destroy(reticle, reticleLifetime);
        }
    }
}