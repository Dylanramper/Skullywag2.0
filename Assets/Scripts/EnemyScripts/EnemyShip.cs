using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float turnSpeed = 60f;
    public int maxHealth = 3;

    [Header("Power-up Spawning")]
    public GameObject shieldPowerUpPrefab;
    public GameObject speedPowerUpPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.4f;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected Transform player;

    [Header("Wander")]
    public float wanderTurnInterval = 2f;
    public float wanderTurnAmount = 60f;

    protected float nextWanderTurnTime;
    protected bool playerDetected;
    public float detectionRange = 6f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void FixedUpdate()
    {
        DetectPlayer(); // updates playerDetected every frame

        if (playerDetected)
        {
            AggroBehavior(); // enemy-specific chasing/firing
        }
        else
        {
            Wander(); // idle movement
        }
    }

    protected virtual void Wander()
    {
        // Only turn at intervals
        if (Time.time >= nextWanderTurnTime)
        {
            // Pick a random turn direction
            float randomTurn = Random.Range(-wanderTurnAmount, wanderTurnAmount);

            // Apply turn to Rigidbody2D
            rb.angularVelocity = randomTurn;

            // Next time to pick a new turn
            nextWanderTurnTime = Time.time + wanderTurnInterval;
        }

        // Keep moving forward slowly
        rb.linearVelocity = transform.up * moveSpeed;
    }

    protected virtual void DetectPlayer()
    {
        if (!player) return; // safety

        float distance = Vector2.Distance(transform.position, player.position);
        playerDetected = distance <= detectionRange;
    }

    // Default empty implementation
    protected virtual void AggroBehavior()
    {
        // Base class does nothing by default
    }

    protected virtual void FacePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, dir);
        rb.angularVelocity = angle * turnSpeed;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        TrySpawnPowerUp();
        Destroy(gameObject);
    }

    protected virtual void TrySpawnPowerUp()
    {
        // Check if we should drop anything
        if (Random.value > dropChance)
        {
            return; // No drop this time
        }

        // Decide WHICH power-up to drop
        GameObject powerUpToDrop = null;

        // Simple 50/50 random choice between the two
        if (Random.value < 0.5f && shieldPowerUpPrefab != null)
        {
            powerUpToDrop = shieldPowerUpPrefab;
            Debug.Log("Enemy dropped a Shield!");
        }
        else if (speedPowerUpPrefab != null)
        {
            powerUpToDrop = speedPowerUpPrefab;
            Debug.Log("Enemy dropped a Speed Boost!");
        }

        // Spawn the chosen power-up
        if (powerUpToDrop != null)
        {
            Instantiate(powerUpToDrop, transform.position, Quaternion.identity);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cannonball"))
        {
            currentHealth -= 1;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

}