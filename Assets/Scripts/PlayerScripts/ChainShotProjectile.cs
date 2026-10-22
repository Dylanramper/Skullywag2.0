using System.Collections;
using UnityEngine;

public class ChainShotProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float travelTime = 1f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Slow Effect")]
    [SerializeField] private float slowAmount = 0.7f;
    [SerializeField] private float slowDuration = 3f;

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
        BrigEnemy brig = collision.GetComponent<BrigEnemy>();

        if (enemy == null && boss == null && brig == null)
            return;

        hasHit = true;

        int damageAmount = Mathf.RoundToInt(damage);

        if(enemy != null)
        {
            enemy.TakeDamage(damageAmount);

            enemy.StartCoroutine(ApplySlow(enemy, brig));
        }
        if (boss != null)
        {
            boss.TakeDamage(damageAmount);
        }
        Destroy(gameObject);
    }

    private IEnumerator ApplySlow(EnemyShip enemy, BrigEnemy brig)
    {
        float originalSpeed = 0f;
        float brigOriginalSpeed = 0f;

        if (enemy != null)
        {
            originalSpeed = enemy.moveSpeed;
            enemy.moveSpeed = originalSpeed * (1f - slowAmount);
        }

        if (brig != null)
        {
            brigOriginalSpeed = brig.broadsideSpeed;
            brig.broadsideSpeed = brigOriginalSpeed * (1f - slowAmount);
        }

        yield return new WaitForSeconds(slowDuration);

        if (enemy != null)
            enemy.moveSpeed = originalSpeed;

        if (brig != null)
            brig.broadsideSpeed = brigOriginalSpeed;
    }


}
