using System.Collections;
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

    [SerializeField] private ParticleSystem leftFX1;
    [SerializeField] private ParticleSystem leftFX2;
    [SerializeField] private ParticleSystem rightFX1;
    [SerializeField] private ParticleSystem rightFX2;

    private float chargeTimer;
    private float nextChargeTime;
    private float nextFireTime;

    [Header("Death VFX")]
    [SerializeField] private ParticleSystem explsion1;
    [SerializeField] private ParticleSystem explsion2;
    [SerializeField] private ParticleSystem explsion3;
    [SerializeField] private ParticleSystem explsion4;
    [SerializeField] private ParticleSystem trail1;
    [SerializeField] private ParticleSystem trail2;
    [SerializeField] private ParticleSystem trail3;
    [SerializeField] private ParticleSystem smokeFX1;
    [SerializeField] private ParticleSystem smokeFX2;
    [SerializeField] private ParticleSystem smokeFX3;

    [SerializeField] private float explosionDelay = 0.5f;
    [SerializeField] private float secondExplosions = 0.4f;
    [SerializeField] private float thirdExplosions = 0.2f;

    protected override void AggroBehavior()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position);
        float distance = toPlayer.magnitude;
        toPlayer.Normalize();

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
        return Mathf.Abs(angleToPlayer) < 30f;
    }

    //Desicion Making
    private void DecideNextAction(Vector2 toPlayer, float distance)
    {
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        //Force turn if player is behind Galleon
        if(Vector2.Dot(transform.up, toPlayer)  < -0.1f)
        {
            rb.MoveRotation(rb.rotation + Mathf.Sign(angleToPlayer) * turnSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = transform.up * moveSpeed;
            return;
        }

        //If far, charge
        if (distance > detectionRange && Time.time >= nextChargeTime && CanCharge(angleToPlayer))
        {
            StartCharge();
            return;
        }

        //If player passes in front, Choose action
        if (Mathf.Abs(angleToPlayer) < 30f)
        {
            if (Random.value > 0.7f && Time.time >= nextChargeTime)
            {
                StartCharge();
            }
            else
            {
                currentState = GalleonState.Broadside;

                return;
            }
        }

        //If the player is closer to the sides, align cannons to the player
        if (distance <= broadsideRange && Mathf.Abs(Mathf.Abs(angleToPlayer) - 90f) < 35f)
        {
            currentState = GalleonState.Broadside;
            return;
        }

        //Default slow turn toward the player
        rb.MoveRotation(rb.rotation + Mathf.Sign(angleToPlayer) * turnSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = transform.up * moveSpeed;

    }

    //Charge Ability
    private void StartCharge()
    {
        currentState = GalleonState.Charging;
        chargeTimer = chargeDuration;
        nextChargeTime = Time.time + chargeCooldown;
        Debug.Log("Charging!");
    }

    private void ChargeBehavior()
    {
        //Lock rotation during charge. Only move straight
        rb.linearVelocity = transform.up * moveSpeed * chargeSpeedMultiplier;

        chargeTimer -= Time.fixedDeltaTime;
        if(chargeTimer <= 0f)
        {
            currentState = GalleonState.Broadside;
        }
    }

    //Broadside (Line up side cannons toward player)
    private void BroadsideBehavior(Vector2 toPlayer)
    {
        Debug.Log("Lining up shot!");
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        float sideAngle = angleToPlayer > 0f ? broadsideAngle : -broadsideAngle;

        float angleDiff = Mathf.DeltaAngle(angleToPlayer, sideAngle);

        //Gentle correction
        rb.MoveRotation(rb.rotation + Mathf.Sign(angleDiff) * turnSpeed * 0.35f * Time.fixedDeltaTime);

        rb.linearVelocity = transform.up * moveSpeed;

        //Fire when cannons align
        if (Time.time >= nextFireTime && Mathf.Abs(angleDiff) <= angleTolerance)
        {
            FireBroadside(sideAngle);
            nextFireTime = Time.time + fireCooldown;
        }

        //Drift out of broadside if player leaves
        if(Mathf.Abs(angleToPlayer) < 40f || Mathf.Abs(angleToPlayer) > 140f)
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

        if(sideAngle >= 0)
        {
            leftFX1.Play();
            leftFX2.Play();
        }else if(sideAngle < 0)
        {
            rightFX1.Play();
            rightFX2.Play();
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if(currentHealth <= 30)
        {
            smokeFX1.Play();
            smokeFX2.Play();
            smokeFX3.Play();
        }else if(currentHealth >= 31)
        {
            smokeFX1.Stop();
            smokeFX2.Stop();
            smokeFX3.Stop();
        }
    }

    protected override void Die()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        //Stop moving
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        trail1.Stop();
        trail2.Stop();
        trail3.Stop();

        //Disable Collider
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col != null)
            col.enabled = false;

        //First Explosion
        if (explsion1 != null)
        {
            explsion1.transform.parent = null;
            explsion1.Play();

            float totalDuration1 = explsion1.main.duration + explsion1.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration1);
        }

        yield return new WaitForSeconds(explosionDelay);

        //Second Explosion
        if (explsion2 != null)
        {
            explsion2.transform.parent = null;
            explsion2.Play();

            float totalDuration2 = explsion2.main.duration + explsion2.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration2);
        }

         yield return new WaitForSeconds(secondExplosions);

        //Third Explosion
        if(explsion3 != null)
        {
            explsion3.transform.parent = null;
            explsion3.Play();

            float totalDuration3 = explsion3.main.duration + explsion3.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration3);
        }

        yield return new WaitForSeconds(thirdExplosions);

        //Forth Explosion
        if (explsion4 != null)
        {
            explsion4.transform.parent = null;
            explsion4.Play();

            float totalDuration4 = explsion4.main.duration + explsion4.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration4);
        }
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
