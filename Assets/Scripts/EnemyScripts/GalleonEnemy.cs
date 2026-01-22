using UnityEngine;

public class GalleonEnemy : EnemyShip
{
    protected override void Awake()
    {
        base.Awake();
        maxHealth = 10;
        currentHealth = maxHealth;
        turnSpeed *= 0.5f;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(Mathf.Max(1, damage, - 1));
    }
}
