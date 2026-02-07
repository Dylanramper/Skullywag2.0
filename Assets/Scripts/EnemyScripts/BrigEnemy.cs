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
    }
}
