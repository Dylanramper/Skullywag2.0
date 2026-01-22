using UnityEngine;

public class BrigEnemy : EnemyShip
{
    public GameObject cannonball;
    public Transform firepoint;
    public float coolDown = 2f;

    private float fireRate;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Fire();
    }

    void Fire()
    {
        if (Time.time < fireRate)
            return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > 6f)
            return;

        fireRate = Time.time + coolDown;

        GameObject ball = Instantiate(cannonball, firepoint.position, firepoint.rotation);
        ball.GetComponent<Rigidbody2D>().linearVelocity = firepoint.up * 5f;
    }
}
