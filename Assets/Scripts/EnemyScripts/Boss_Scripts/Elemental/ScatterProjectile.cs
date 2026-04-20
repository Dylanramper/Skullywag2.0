using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterProjectile : MonoBehaviour
{
    [Header("Travel")]
    public float travelTime = 0.8f;
    public float minScale = 0.5f;
    public float maxScale = 1.2f;

    [Header("Explosion")]
    public GameObject explosionFX;

    private Vector2 startPos;
    private Vector2 targetPos;

    private List<Vector2> targetPoints;

    public void Initialize(Vector2 start, Vector2 target, List<Vector2> reticleTargets)
    {
        startPos = start;
        targetPos = target;
        targetPoints = reticleTargets;

        StartCoroutine(Travel());
    }

    IEnumerator Travel()
    {
        float time = 0f;

        while (time < travelTime)
        {
            float t = time / travelTime;

            // Move
            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Scale up (simulate going up)
            float scale = Mathf.Lerp(minScale, maxScale, t);
            transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        Instantiate(explosionFX, transform.position, Quaternion.identity);

        Debug.Log("ScatterProjectile exploded — next step: spawn ScatterShotBalls");

        // We'll add the actual scatter shots NEXT step

        Destroy(gameObject);
    }
}