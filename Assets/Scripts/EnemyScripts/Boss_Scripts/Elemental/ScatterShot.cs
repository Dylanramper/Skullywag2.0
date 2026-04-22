using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterShot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Timing")]
    public float delayBetweenShots = 0.05f;

    private List<Vector2> targetPosition;

    public void Initialize(List<Vector2> targets)
    {
        targetPosition = targets;
        StartCoroutine(FireScatter());
    }

    private IEnumerator FireScatter()
    {
        foreach (var target in targetPosition)
        {
            Vector3 spawnPos = transform.position;

            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Vector2 direction = (target - (Vector2)spawnPos).normalized;
                rb.linearVelocity = direction * projectileSpeed;
            }

            yield return new WaitForSeconds(delayBetweenShots);
        }
    }
}