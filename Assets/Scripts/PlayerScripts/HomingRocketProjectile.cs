
using System.Collections;
using UnityEngine;

public class HomingRocketProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float launchSpeed = 4f;
    [SerializeField] private float launchDuration = 0.25f;
    [SerializeField] private float slowSpeed = 6f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float turnDuration = 0.2f;

    [Header("Damage")]
    [SerializeField] private float damage = 20f;

    [Header("Lifetime")]
    [SerializeField] private float maxLifetime = 6f;

    [Header("Impact Effects")]
    [SerializeField] private GameObject explosionPrefab;

    private Transform target;
    private float currentSpeed;
    private bool isHoming;
    private bool hasHit;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Start()
    {
        currentSpeed = launchSpeed;
        StartCoroutine(LockOnRoutine());
        Destroy(gameObject, maxLifetime);
    }

    private void Update()
    {
        if (hasHit)
            return;

        if (isHoming)
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector2 directionToTarget = (Vector2)target.position - (Vector2)transform.position;

            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                float angle = Vector2.SignedAngle(transform.up, directionToTarget.normalized);

                float turnAmount = Mathf.Clamp(angle, -turnSpeed * Time.deltaTime, turnSpeed * Time.deltaTime);

                transform.Rotate(0f, 0f, turnAmount);
            }
        }

        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    private IEnumerator LockOnRoutine()
    {
        // Phase 1: Push outward before turning.
        yield return new WaitForSeconds(launchDuration);

        // Phase 2: Slow down slightly while locking on.
        currentSpeed = slowSpeed;

        float elapsed = 0f;

        while (elapsed < turnDuration)
        {
            if (target == null)
            {
                Destroy(gameObject);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Phase 3: Accelerate and pursue the target.
        currentSpeed = moveSpeed;
        isHoming = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit || target == null)
            return;

        EnemyShip enemy = collision.GetComponentInParent<EnemyShip>();
        BaseBoss boss = collision.GetComponentInParent<BaseBoss>();

        bool hitTarget = false;

        if (enemy != null)
        {
            hitTarget = enemy.transform == target || enemy.transform.IsChildOf(target) || target.IsChildOf(enemy.transform);
        }
        else if (boss != null)
        {
            hitTarget = boss.transform == target || boss.transform.IsChildOf(target) || target.IsChildOf(boss.transform);
        }

        if (!hitTarget)
            return;

        hasHit = true;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosion();
        }

        int damageAmount = Mathf.RoundToInt(damage);

        if (enemy != null)
            enemy.TakeDamage(damageAmount);
        else if (boss != null)
            boss.TakeDamage(damageAmount);

        Destroy(gameObject);
    }
}
