using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class RowboatEnemy : EnemyShip
{
    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Wander")]
    public float wanderTurnInterval = 2f; //Change to random.range();
    public float wanderTurnAmount = 45f;
    private float nextWanderTurnTime;
    private bool playerDetected;

    [Header("Explosion")]
    public float fuseTime = 1.5f;
    public float explosionRadius = 1.5f;
    public int explosionDamage = 1;
    public LayerMask damageLayers;

    private bool fuseStarted;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void FixedUpdate()
    {
        DetectPlayer();

        if (playerDetected)
        {
            ChasePlayer();
        }
        else { Wander(); }

        MoveForward();
    }
    void DetectPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        playerDetected = distance <= detectionRange;
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, direction);

        float turn = Mathf.Clamp(angle, -1f, 1f);
        rb.angularVelocity = turn * turnSpeed;
    }

    void Wander()
    {
        if(Time.time >= nextWanderTurnTime)
        {
            float randomTurn = Random.Range(-wanderTurnAmount, wanderTurnAmount);
            rb.angularVelocity = randomTurn;
            nextWanderTurnTime = Time.time + wanderTurnInterval;
        }
    }

    protected override void Die()
    {
        if(!fuseStarted)
        {
            Explode();
        }
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Playerhit");
            }
        }
        Destroy(gameObject);
    }

    IEnumerator FuseAndExplosion()
    {
        fuseStarted = true;

        float timer = 0f;
        Color originalColor = spriteRenderer.color;

        while (timer < fuseTime)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }
        Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (fuseStarted)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FuseAndExplode());
        }
    }
}
