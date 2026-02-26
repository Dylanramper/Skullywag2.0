using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BrigEnemy : EnemyShip
{
    [Header("Brig Movement")]
    public float idealRange = 6f;
    public float tooCloseRange = 3.5f;
    public float broadsideSpeed = 1.5f;

    [Header("Cannons")]
    public Transform leftCannon;
    public Transform rightCannon;
    public GameObject cannonballPrefab;

    [Header("Broadside Firing")]
    public float broadsideAngle = 45f;
    public float angleTolerance = 5f;
    public float fireCooldown = 2f;

    private float nextFireTime;

    [Header("Death VFX")]
    [SerializeField] private ParticleSystem explsion1;
    [SerializeField] private ParticleSystem explsion2;
    [SerializeField] private ParticleSystem explsion3;
    [SerializeField] private ParticleSystem trail1;
    [SerializeField] private ParticleSystem trail2;
    [SerializeField] private ParticleSystem leftCannonFX;
    [SerializeField] private ParticleSystem rightCannonFX;
    [SerializeField] private ParticleSystem SmokeFX1;
    [SerializeField] private ParticleSystem SmokeFX2;

    [SerializeField] private float explosionDelay = 0.5f;
    [SerializeField] private float secondDelay = 0.3f;
    

    protected override void AggroBehavior()
    {
        Vector2 toPlayer = ((Vector2) player.position - rb.position).normalized;
        float distance = Vector2.Distance(rb.position, player.position);

        //Always drift forward
        rb.linearVelocity = transform.up * broadsideSpeed;

        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);
        float turnDirection = Mathf.Sign(angleToPlayer);

        //if too far, give chase
        if(distance > tooCloseRange)
        {
            rb.MoveRotation(rb.rotation + turnDirection * turnSpeed * 0.6f * Time.fixedDeltaTime);
        }
        else
        {
            MaintainBroadside(angleToPlayer);
        }
    }
    //Maintain angle during combat.
    private void MaintainBroadside(float angleToPlayer)
    {
        //Pick the closest side
        float targetAngle = Mathf.Abs(angleToPlayer - broadsideAngle) < Mathf.Abs(angleToPlayer + broadsideAngle) ? broadsideAngle : -broadsideAngle;
        float angleDiff = Mathf.DeltaAngle(angleToPlayer, targetAngle);

        //Make correction
        rb.MoveRotation(rb.rotation - Mathf.Sign(angleDiff) * (turnSpeed * 0.3f) * Time.fixedDeltaTime);

        TryFireBroadside(angleDiff, targetAngle);
    }

    private void TryFireBroadside(float angleDiff, float targetAngle)
    {
        if (Time.time < nextFireTime) return;
        if (Mathf.Abs(angleDiff) > angleTolerance) return;

        FireBroadside(targetAngle);
        nextFireTime = Time.time + fireCooldown;
    }

    private void FireBroadside(float sideAngle)
    {
        Transform cannonToFire = sideAngle > 0 ? leftCannon : rightCannon;

        if(cannonToFire == null || cannonballPrefab == null)
        {
            Debug.LogWarning("Brig cannon setup missing!");
            return;
        }

        Instantiate(cannonballPrefab, cannonToFire.position, cannonToFire.rotation);

        if(sideAngle >= 0)
        {
            leftCannonFX.Play();
        }else if(sideAngle < 0)
        {
            rightCannonFX.Play();
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if(currentHealth <= 20)
        {
            SmokeFX1.Play();
            SmokeFX2.Play();
        }else if(currentHealth >= 21)
        {
            SmokeFX1.Stop();
            SmokeFX2.Stop();
        }
    }

    protected override void Die()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        //Stop moving
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;

        trail1.Stop();
        trail2.Stop();

        //Disable Collider
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if(col != null)
           col.enabled = false;

        enabled = false;

        //First Explosion
        if(explsion1 != null)
        {
            explsion1.transform.parent = null;
            explsion1.Play();

            float totalDuration1 = explsion1.main.duration + explsion1.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration1);
        }

        yield return new WaitForSeconds(explosionDelay);
        
        //Second Explosion
        if(explsion2 != null)
        {
            explsion2.transform.parent = null;
            explsion2.Play();

            float totalDuration2 = explsion2.main.duration + explsion2.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration2);
        }

        yield return new WaitForSeconds(secondDelay);

        //Third Explosion
        if(explsion3 != null)
        {
            explsion3.transform.parent = null;
            explsion3.Play();

            float totalDuration3 = explsion3.main.duration + explsion3.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration3);
        }

        Destroy(gameObject);
    }
}
