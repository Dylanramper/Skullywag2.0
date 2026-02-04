using UnityEngine;

public class RowboatEnemy : EnemyShip
{
    protected override void AggroBehavior()
    {
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;

        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        rb.MoveRotation(rb.rotation + angleToPlayer * turnSpeed * Time.fixedDeltaTime / 90f);
        rb.linearVelocity = transform.up * moveSpeed;
    }

    protected override void Die()
    {
        //TODO: Explosion + Damage
        Destroy(gameObject);
    }
}
