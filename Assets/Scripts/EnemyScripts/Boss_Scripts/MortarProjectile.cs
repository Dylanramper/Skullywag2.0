using UnityEngine;
using System.Collections;

public class MortarProjectile : MonoBehaviour
{
    public float travelTime = 1f;
    public float minScale = 0.5f;   // start & end size
    public float maxScale = 1f;   // peak size
    public GameObject explosionPrefab;

    private Vector2 startPos;
    private Vector2 targetPos;

    private void FixedUpdate()
    {
        transform.Rotate(0f, 0f, 720f * Time.deltaTime);
    }
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

            // Move toward target
            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Fake height using scale
            float scale;

            if (t < 0.5f)
            {
                // Going up
                scale = Mathf.Lerp(minScale, maxScale, t * 2f);
            }
            else
            {
                // Coming down
                scale = Mathf.Lerp(maxScale, minScale, (t - 0.5f) * 2f);
            }

            transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }

        // Snap to exact target
        transform.position = targetPos;

        Instantiate(explosionPrefab, targetPos, Quaternion.identity);
        AudioManager.Instance.PlayExplosion();

        Destroy(gameObject);
    }
}