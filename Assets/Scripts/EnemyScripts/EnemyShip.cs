using UnityEngine;

public abstract class EnemyShip : MonoBehaviour
{
    [Header("Base Stats")]
    public float moveSpeed = 2f;
    public float turnSpeed = 120f;
    public float detectionRange = 8f;
    public int maxHealth = 1;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected Transform player;
    protected bool playerDetected;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
    }

    protected virtual void FixedUpdate()
    {
        CheckPlayerDetection();
        if (playerDetected)
        {
            AggroBehavior();
        }
        else { Wander(); }
    }

    protected void CheckPlayerDetection()
    {
        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        playerDetected = distance <= detectionRange;
    }

    protected virtual void Wander()
    {
        rb.linearVelocity = transform.up * moveSpeed * 0.5f;
    }

    protected abstract void AggroBehavior();

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
