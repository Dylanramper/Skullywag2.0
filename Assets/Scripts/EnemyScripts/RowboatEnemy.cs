using UnityEngine;

public class RowboatEnemy : EnemyShip
{
    public float chargeSpeed = 4f;

    protected override void MoveForward()
    {
        rb.linearVelocity = transform.up * chargeSpeed;
    }
}
