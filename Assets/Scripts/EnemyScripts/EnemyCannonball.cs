using UnityEngine;

public class EnemyCannonball : MonoBehaviour
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
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.ShakeCamera(0.2f, 0.1f);
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
