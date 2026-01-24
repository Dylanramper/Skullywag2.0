using UnityEngine;

public class RowboatEnemy : EnemyShip
{
    [Header("Detection")]
    public float detectionRange = 6f;

    [Header("Wander")]
    public float wanderTurnInterval = 2f; //Change to random.range();
    public float wanderTurnAmount = 45f;

    private float nextWanderTurnTime;
    private bool playerDetected;

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
}
