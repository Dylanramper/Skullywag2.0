using UnityEngine;

public class GalleonEnemy : EnemyShip
{
    private enum GalleonState { Wandering, Charging, Broadside };
    private GalleonState currentState = GalleonState.Wandering;

    [Header("Charge Settings")]
    private float chargeSpeedMultiplier = 1.5f;
    private float chargeDuration = 2.5f;
    public float chargeCooldown = 4f;

    [Header("Broadside Settings")]
    public float broadsideRange = 4f;
    public float broadsideAngle = 90f;
    public float angleTolerance = 10f;
    public float fireCooldown = 3f;

    [Header("Cannons")]
    public Transform[] leftCannons;
    public Transform[] rightCannons;
    public GameObject cannonballPrefab;

    private float chargeTimer;
    private float nextChargeTime;
    private float nextFireTime;

    protected override void AggroBehavior()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position);
        float distance = toPlayer.magnitude;
        toPlayer.Normalize();

        rb.linearVelocity = transform.up * moveSpeed;

        switch (currentState)
        {
            case GalleonState.Wandering:
                DecideNextAction(toPlayer, distance);
                break;

            case GalleonState.Charging:
                ChargeBehavior();
                break;
            case GalleonState.Broadside:
                BroadsideBehavior(toPlayer);
                break;
        }
    }

    private bool CanCharge(float angleToPlayer)
    {
        // Only allow charge if player is mostly in front
        return Mathf.Abs(angleToPlayer) < 35f;
    }

    //Desicion Making
    private void DecideNextAction(Vector2 toPlayer, float distance)
    {
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        //If far, charge
        if (distance > detectionRange && Time.time >= nextChargeTime && CanCharge(angleToPlayer))
        {
            StartCharge();
            return;
        }

        //If player passes in front, Choose action
        if (Mathf.Abs(angleToPlayer) < 30f)
        {
            if (Random.value > 0.5f && Time.time >= nextChargeTime)
            {
                StartCharge();
            }
            else currentState = GalleonState.Broadside;

            return;
        }

        //If the player is closer to the sides, align cannons to the player
        if (distance <= broadsideRange && Mathf.Abs(Mathf.Abs(angleToPlayer) - 90f) < 35f)
        {
            currentState = GalleonState.Broadside;
            return;
        }

        //Default slow turn toward the player
        rb.MoveRotation(rb.rotation - Mathf.Sign(angleToPlayer) * turnSpeed * 0.4f * Time.fixedDeltaTime);
    }

    //Charge Ability
    private void StartCharge()
    {
        currentState = GalleonState.Charging;
        chargeTimer = chargeDuration;
        nextChargeTime = Time.time + chargeCooldown;
    }

    private void ChargeBehavior()
    {
        //Lock rotation during charge. Only move straight
        rb.linearVelocity = transform.up * moveSpeed * chargeSpeedMultiplier;

        chargeTimer -= Time.fixedDeltaTime;
        if(chargeTimer <= 0f)
        {
            currentState = GalleonState.Wandering;
        }
    }

    //Broadside (Line up side cannons toward player)
    private void BroadsideBehavior(Vector2 toPlayer)
    {
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        float targetAngle = Mathf.Abs(angleToPlayer - broadsideAngle) < Mathf.Abs(angleToPlayer + broadsideAngle) ? broadsideAngle : -broadsideAngle;

        float angleDiff = Mathf.DeltaAngle(angleToPlayer, targetAngle);

        //Gentle correction
        rb.MoveRotation(rb.rotation - Mathf.Sign(angleDiff) * turnSpeed * 0.35f * Time.fixedDeltaTime);

        //Fire when cannons align
        if (Time.time >= nextFireTime && Mathf.Abs(angleDiff) <= angleTolerance)
        {
            FireBroadside(targetAngle);
            nextFireTime = Time.time + fireCooldown;
        }

        //Drift out of broadside if player leaves
        if(Mathf.Abs(angleToPlayer) < 40f)
        {
            currentState = GalleonState.Wandering;
        }
    }

    //Fire cannons
    private void FireBroadside(float sideAngle)
    {
        Transform[] cannons = sideAngle > 0 ? leftCannons : rightCannons;
        if (cannons == null || cannonballPrefab == null) return;

        foreach(Transform cannon in cannons)
        {
            Instantiate(cannonballPrefab, cannon.position, cannon.rotation);
        }
    }

    protected override void Die()
    {
        //TODO: Death animation or sinking?
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, broadsideRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, loseAggroRange);
    }
}
