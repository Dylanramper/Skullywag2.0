using UnityEngine;

public class CannonBallProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Try to damage enemy
            EnemyShip enemy = other.GetComponent<EnemyShip>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Destroy the cannon ball
            Destroy(gameObject);
        }

        // Optional: Also destroy when hitting walls/obstacles
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}