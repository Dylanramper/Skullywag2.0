using UnityEngine;

public class RowboatEnemy : EnemyShip
{
    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Wander")]
    public float wanderTurnInterval = 2f;
    public float wanderTurnAmount = 45f;

    [Header("Power-up Spawning")]
    public GameObject shieldPowerUpPrefab; // Drag shield prefab here
    [Range(0f, 1f)]
    public float dropChance = 0.3f; // 30% chance to drop

    private float nextWanderTurnTime;
    private bool playerDetected;

    void Start()
    {
        // Initialize next wander time
        nextWanderTurnTime = Time.time + Random.Range(0f, wanderTurnInterval);
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
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerDetected = distance <= detectionRange;
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, direction);

        float turn = Mathf.Clamp(angle, -1f, 1f);
        rb.angularVelocity = turn * turnSpeed;
    }

    void Wander()
    {
        if (Time.time >= nextWanderTurnTime)
        {
            float randomTurn = Random.Range(-wanderTurnAmount, wanderTurnAmount);
            rb.angularVelocity = randomTurn;
            nextWanderTurnTime = Time.time + wanderTurnInterval;
        }
    }

    // This is called when enemy dies (from parent class or however your system works)
    protected override void Die()
    {
        // Try to spawn shield power-up
        TrySpawnShield();

        // Call base method if it exists
        base.Die();
    }

    void TrySpawnShield()
    {
        // Check if we have a shield prefab and random chance succeeds
        if (shieldPowerUpPrefab != null && Random.value <= dropChance)
        {
            // Spawn shield at enemy position
            Instantiate(shieldPowerUpPrefab, transform.position, Quaternion.identity);
            Debug.Log("Enemy dropped a shield!");
        }
    }
}