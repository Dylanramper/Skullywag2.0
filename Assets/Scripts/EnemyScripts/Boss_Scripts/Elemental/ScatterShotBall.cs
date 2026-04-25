using System.Collections;
using UnityEngine;

public class ScatterShotBall : MonoBehaviour
{
    public float travelTime = 0.5f;
    public float startScale = 1f;
    public float endScale = 0.1f;

    public GameObject impactFX;

    private Vector2 startPos;
    private Vector2 targetPos;

    public void Launch(Vector2 start, Vector2 target)
    {
        startPos = start;
        targetPos = target;

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float time = 0f;

        while (time < travelTime)
        {
            float t = time / travelTime;

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Shrink = falling illusion
            float scale = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }

        Impact();
    }

    void Impact()
    {
        Instantiate(impactFX, targetPos, Quaternion.identity);
        Destroy(gameObject);
    }
}