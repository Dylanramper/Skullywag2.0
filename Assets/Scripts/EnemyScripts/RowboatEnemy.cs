using UnityEngine;

public class RowboatEnemy : EnemyShip
{
    public static GamePowerUpManager Instance;

    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Wander")]
    public float wanderTurnInterval = 2f;
    public float wanderTurnAmount = 45f;

    [Header("Power-up Spawning")]
    public PowerUpSpawner powerUpSpawner;
    [Tooltip("Override spawner position if needed")]
    public Transform spawnPointOverride;

    private float nextWanderTurnTime;
    private bool playerDetected;

    void Start()
    {
        // Register with PowerUpManager if it exists
        if (PowerUpManager.Instance != null && powerUpSpawner != null)
        {
            PowerUpManager.Instance.RegisterEnemy(gameObject, powerUpSpawner);
        }

        // Position spawner at override point or use enemy position
        if (spawnPointOverride != null && powerUpSpawner != null)
        {
            powerUpSpawner.transform.position = spawnPointOverride.position;
        }
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
        if (Time.time >= nextWanderTurnTime)
        {
            float randomTurn = Random.Range(-wanderTurnAmount, wanderTurnAmount);
            rb.angularVelocity = randomTurn;
            nextWanderTurnTime = Time.time + wanderTurnInterval;
        }
    }

    // Called when enemy is destroyed
    protected override void Die()
    {
        // Notify PowerUpManager
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.OnEnemyDestroyed(gameObject);
        }
        // Or use local spawner if manager doesn't exist
        else if (powerUpSpawner != null)
        {
            powerUpSpawner.SpawnPowerUp();
        }

        base.Die();
    }
}