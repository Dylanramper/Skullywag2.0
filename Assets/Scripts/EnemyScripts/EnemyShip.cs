using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float turnSpeed = 60f;
    public int maxHealth = 3;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected Transform player;
    
    //Get the components and set health to max health. 
    //Find player with the 'Player' tag.
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void FixedUpdate()
    {
        MoveForward();
    }
    
    protected virtual void MoveForward()
    {
        rb.linearVelocity = transform.up * moveSpeed;
    }

    protected virtual void FacePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, dir);
        rb.angularVelocity = angle * turnSpeed;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

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
