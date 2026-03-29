using System.Collections;
using UnityEngine;

public abstract class EnemyShip : MonoBehaviour
{
    [Header("Hit Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color originalColor;

    [Header("Wander")]
    [SerializeField] protected float wanderTurnStrength = 20f;
    [SerializeField] protected float wanderChangeInterval = 2f;

    protected float wanderTurnDirection;
    protected float nextWanderChangeTime;

    [Header("Base Stats")]
    public float moveSpeed = 2f;
    public float turnSpeed = 120f;
    public float detectionRange = 8f;
    public int maxHealth;

    protected int currentHealth;
    protected Rigidbody2D rb;
    protected Transform player;
    protected bool playerDetected;

    [Header("Aggro Settings")]
    public float loseAggroRange = 14f;

    [SerializeField] FloatingHealthbar healthbar;

    protected virtual void Awake()
    {
        healthbar = GetComponentInChildren<FloatingHealthbar>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        healthbar.UpdateHealthbar(currentHealth, maxHealth);
    }

    protected virtual void FixedUpdate()
    {
        CheckPlayerDetection();

        if (playerDetected)
        {
            rb.linearVelocity = Vector2.zero;
            AggroBehavior();
            AudioManager.Instance.PlayMusic(combatMusic);
        }
        else
        {
            Wander();
            AudioManager.Instance.PlayMusic(gameplayMusic);
        }
    }

    protected void CheckPlayerDetection()
    {
        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        if (playerDetected)
        {
            //Disengage if too far
            if (distance > loseAggroRange)
            {
                playerDetected = false;
            }
        }
        else
        {
            //If not Aggro'd and player is in detection range. Detected set to true.
            if (distance < detectionRange)
                playerDetected = true;
        }
    }

    protected virtual void Wander()
    {
        //Always move forward
        rb.linearVelocity = transform.up * moveSpeed * 0.5f;

        //Occasionally turn
        if(Time.time > nextWanderChangeTime)
        {
            int choice = Random.Range(0, 3); //0 = left : 2 = right
            wanderTurnDirection = choice == 0 ? -1 : choice == 2 ? 1f : 0f;
            nextWanderChangeTime = Time.time + wanderChangeInterval;
        }

        //Apply turn
        rb.MoveRotation(rb.rotation + wanderTurnDirection * wanderTurnStrength * Time.fixedDeltaTime);
    }

    protected abstract void AggroBehavior();

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        AudioManager.Instance.PlayHit();
        StartCoroutine(Flash());
        GameManager.Instance.ShakeCamera(0.1f, 0.08f);
        if (currentHealth <= 0)
        {
            Die();
        }
        healthbar.UpdateHealthbar(currentHealth, maxHealth);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = originalColor;
    }
}
