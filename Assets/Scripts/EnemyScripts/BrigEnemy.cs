using Unity.VisualScripting;
using UnityEngine;

public class BrigEnemy : EnemyShip
{
    public float broadsideRange = 6f;
    public float broadsideAngle = 45f;
    public float fireCooldown = 2f;

    float nextFireTime;

    protected override void AggroBehavior()
    {
        float distance = Vector2.Distance(rb.position, player.position);

        if (distance > broadsideRange)
        {
            ChasePlayer();
        }
        else { BroadsidePlayer(); }
    }

    void ChasePlayer()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = transform.up * moveSpeed;

        float angle = Vector2.SignedAngle(transform.up, toPlayer);
        rb.MoveRotation(rb.rotation - Mathf.Sign(angle) * turnSpeed * Time.fixedDeltaTime);
    }

    void BroadsidePlayer()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        float targetAngle = angleToPlayer > 0 ? broadsideAngle : -broadsideAngle;
        float angleDifference = Mathf.DeltaAngle(angleToPlayer, targetAngle);

        rb.MoveRotation(rb.rotation - Mathf.Sign(angleDifference) * turnSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = transform.up * moveSpeed;

        if (Time.time >= nextFireTime && Mathf.Abs(angleDifference) < 5f)
        {
            FireBroadside();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void FireBroadside()
    {
        //Shoot at player
    }
}
