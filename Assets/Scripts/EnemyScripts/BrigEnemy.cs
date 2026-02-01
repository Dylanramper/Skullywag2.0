using UnityEngine;

public class BrigEnemy : EnemyShip
{
    [Header("Brig Stats")]
    public float broadsideSpeed = 2f;

    public float attackRange = 4f;
    public float broadsideAngle = 45f;
    public float angleTolerance = 10f;
    private bool inBroadsideMode;
    public float engageBroadsideRange = 5f;
    public float disengageBroadsideRange = 7f;

    public float fireCooldown = 2f;
    private float nextFireTime;

    public Transform[] leftCannon;
    public Transform[] rightCannon;
    public GameObject cannonballPrefab;

    protected override void Awake()
    {
        base.Awake();      // initializes base class fields
        turnSpeed = 120f;   // set Brig-specific speed
    }
    protected override void AggroBehavior()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Enter broadside mode
        if (!inBroadsideMode && distance <= engageBroadsideRange)
        {
            inBroadsideMode = true;
        }

        // Exit broadside mode
        if (inBroadsideMode && distance >= disengageBroadsideRange)
        {
            inBroadsideMode = false;
        }

        if (inBroadsideMode)
        {
            BroadsidePlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    #region Movement
    private void ChasePlayer()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, toPlayer);

        float turnDirection = Mathf.Sign(angle);

        rb.MoveRotation(
            rb.rotation + turnDirection * turnSpeed * Time.fixedDeltaTime
        );

        rb.linearVelocity = transform.up * moveSpeed;
    }

    private void BroadsidePlayer()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, toPlayer);

        //Decide which side to broadside
        float targetAngle = Mathf.Abs(angle - broadsideAngle) < Mathf.Abs(angle + broadsideAngle) ? broadsideAngle : -broadsideAngle;

        float angleDifference = Mathf.DeltaAngle(angle, targetAngle);
        float turnDirection = Mathf.Sign(angleDifference);

        //Rotate towards broadside
        rb.MoveRotation(rb.rotation + turnDirection * turnSpeed * Time.fixedDeltaTime);

        //Drift forward
        rb.linearVelocity = transform.up * broadsideSpeed;

        //Fire if aligned and cooldown allows
        if(Time.time >= nextFireTime && (Mathf.Abs(angle) - 45f) < angleTolerance)
        {
            FireBroadside(angle);
            nextFireTime = Time.time + fireCooldown;
        }
    }

    #endregion

    #region Firing

    private void FireBroadside(float angle) 
    {
        if(angle > 0)
        {
            foreach (Transform cannon in leftCannon)
                Instantiate(cannonballPrefab, cannon.position, cannon.rotation);
        }
        else
        {
            foreach (Transform cannon in rightCannon)
                Instantiate(cannonballPrefab, cannon.position, cannon.rotation);
        }
    }

    #endregion
}
