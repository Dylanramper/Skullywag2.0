using UnityEngine;
using System.Collections;

public class ScatterProjectile : MonoBehaviour
{
    [Header("Travel Settings")]
    public float travelTime = 0.8f;
    public float minScale = 0.4f;
    public float maxScale = 1f;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public int scatterCount = 6;
    public float scatterRadius = 2.5f;

    public GameObject scatterShotPrefab;
    public GameObject scatterProjectilePrefab;

    private Vector2 startPos;
    private Vector2 targetPos;

    public void Launch(Vector2 start, Vector2 target)
    {
        startPos = start;
        targetPos = target;

        StartCoroutine(Travel());
    }

    IEnumerator Travel()
    {
        float time = 0f;

        while (time < travelTime)
        {
            float t = time / travelTime;

            // Move in arc (simple lerp for top-down fake arc)
            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Fake vertical motion using scale
            float scale = Mathf.Sin(t * Mathf.PI); // smooth up & down
            transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, scale);

            time += Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        Instantiate(explosionPrefab, targetPos, Quaternion.identity);
        AudioManager.Instance.PlayExplosion();

        // Spawn scatter shots
        for (int i = 0; i < scatterCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * scatterRadius;
            Vector2 hitPoint = targetPos + offset;

            //GameObject proj = Instantiate(explosionPrefab, targetPos, Quaternion.identity);

            // If you instead have a "small projectile prefab", swap this line:
            GameObject proj = Instantiate(scatterProjectilePrefab, targetPos, Quaternion.identity);
            GameObject ball = Instantiate(scatterShotPrefab, targetPos, Quaternion.identity);

            // Optional: if scatter is ALSO a projectile, you'd call Launch again
        }

        Destroy(gameObject);
    }
}