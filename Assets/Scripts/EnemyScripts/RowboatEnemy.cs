using System.Collections;
using UnityEngine;

public class RowboatEnemy : EnemyShip
{
    [Header("Explosion Settings")]
    public float explodeDelay = 1.2f;
    public float flashInterval = 0.15f;

    private SpriteRenderer sprite;
    private bool isExploding;

    [Header("Explosion Damage")]
    public float explosionRadius = 3.2f;
    public int explosionDamage = 20;
    public LayerMask damageLayers;

    [Header("Explosion FX")]
    [SerializeField]
    private ParticleSystem explosion;
    [SerializeField]
    private ParticleSystem debris;

    protected override void Awake()
    {
        base.Awake();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    protected override void AggroBehavior()
    {
        if (isExploding) return;

        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        rb.MoveRotation(rb.rotation + angleToPlayer * turnSpeed * Time.fixedDeltaTime / 90f);
        rb.linearVelocity = transform.up * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isExploding) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            StartExplosionCountdown();
        }
    }

    public override void TakeDamage(int amount)
    {
        if(isExploding) return;
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        if (isExploding) return;
        isExploding = true;
        Explode();
    }

    private void StartExplosionCountdown()
    {
        isExploding = true;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        float timer = 0f;
        bool flashOn = false;

        while (timer < explodeDelay)
        {
            flashOn = !flashOn;
            sprite.color = flashOn ? Color.red : Color.white;

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }
        Explode();
    }

    private void PlayExplosionFX()
    {
        if(explosion != null)
        {
            explosion.transform.parent = null;
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration + explosion.main.startLifetime.constantMax);
        }
        if(debris != null)
        {
            debris.transform.parent = null;
            debris.Play();
            Destroy(debris.gameObject, debris.main.duration + debris.main.startLifetime.constantMax);
        }
    }

    private void Explode()
    {
        PlayExplosionFX();

        Collider2D[] hits = Physics2D.OverlapCircleAll(rb.position, explosionRadius, damageLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if(playerHealth != null)
                {
                    playerHealth.TakeDamage(explosionDamage);
                }
            }
            //Damage other enemies
            EnemyShip enemy = hit.GetComponent<EnemyShip>();
            if(enemy != null && enemy != this)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
        //TODO: Damage, sound----------------------------------------------------------------------
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
