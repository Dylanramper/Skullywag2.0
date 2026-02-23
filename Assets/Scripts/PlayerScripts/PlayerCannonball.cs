using UnityEngine;

public class PlayerCannonball : MonoBehaviour
{
    public int damage = 10;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyShip enemy = collision.gameObject.GetComponent<EnemyShip>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log(damage);
            }
            Destroy(gameObject);
        }
    }
}