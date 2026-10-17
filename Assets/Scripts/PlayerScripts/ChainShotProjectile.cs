using System.Collections;
using UnityEngine;

public class ChainShotProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float travelTime = 1f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Slow Effect")]
    [SerializeField] private float slowAmount = 1f;
    [SerializeField] private float slowDuration = 5f;

    private Vector2 startPos;
    private Vector2 targetPos;
    private bool hasHit;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void Launch(Vector2 start, Vector2 target)
    {
        startPos = start;
        targetPos = target;

        StartCoroutine(Travel());
    }

    private IEnumerator Travel()
    {
        float time = 0f;

        while (time < travelTime)
        {
            float t = time / travelTime;
            transform.position = Vector2.Lerp(startPos, targetPos, t);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        if(!hasHit)
        {
            // If it didn't hit anything, destroy the projectile
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
            return;

        EnemyShip enemy = collision.GetComponent<EnemyShip>();
        BaseBoss boss = collision.GetComponent<BaseBoss>();

        if (enemy == null && boss == null)
            return;

        hasHit = true;

        int damageAmount = Mathf.RoundToInt(damage);

        if(enemy != null)
        {
            enemy.TakeDamage(damageAmount);

            enemy.StartCoroutine(ApplySlow(enemy));
        }
        if(boss != null)
        {
            boss.TakeDamage(damageAmount);
        }
        Destroy(gameObject);
    }

    private IEnumerator ApplySlow(EnemyShip enemy)
    {
        float originalSpeed = enemy.moveSpeed;
        enemy.moveSpeed -= slowAmount;
        yield return new WaitForSeconds(slowDuration);
        enemy.moveSpeed = originalSpeed;
    }
}
