using System.Collections;
using UnityEngine;

public class ScatterShot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Scatter Settings")]
    public int projectileCount = 8;
    public float scatterRadius = 3f;
    public float spawnHeight = 8f;
    public float damage = 10f;

    [Header("Timing")]
    public float delayBetweenShots = 0.1f;

    private Transform player;

    public void Initialize(Transform targetPlayer)
    {
        player = targetPlayer;
        StartCoroutine(FireScatter());
    }

    private IEnumerator FireScatter()
    {
        Vector3 baseTarget = player.position;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * scatterRadius;

            Vector3 spawnPos = baseTarget + new Vector3(randomOffset.x, spawnHeight, randomOffset.y);
            Vector3 hitPoint = baseTarget + new Vector3(randomOffset.x, 0f, randomOffset.y);

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (hitPoint - spawnPos).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }
    }
}