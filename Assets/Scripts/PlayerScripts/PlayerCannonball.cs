using UnityEngine;

public class PlayerCannonball : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 3f;
    public float speed = 7f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Fire forward in the cannon's facing direction
        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyShip enemy = other.GetComponent<EnemyShip>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}