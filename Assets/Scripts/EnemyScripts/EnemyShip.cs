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

    public static int activeEnemiesInCombat = 0;
    private bool countedInCombat = false;

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
        Debug.Log("Enemies in combat: " + activeEnemiesInCombat);
        CheckPlayerDetection();

        if (playerDetected)
        {
            rb.linearVelocity = Vector2.zero;
            AggroBehavior();
        }
        else
        {
            Wander();
        }
    }

    protected void CheckPlayerDetection()
    {
        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        if (playerDetected)
        {
            if (activeEnemiesInCombat <= 0)
            {
                activeEnemiesInCombat = 0;
                AudioManager.Instance.PlayGameplayMusic();
            }

            //Disengage if too far
            if (distance > loseAggroRange)
            {
                playerDetected = false;

                if (countedInCombat)
                {
                    countedInCombat = false;
                }
            }
        }
        else
        {
            //If not Aggro'd and player is in detection range. Detected set to true.
            if (distance < detectionRange)
            {
                playerDetected = true;

                if (!countedInCombat)
                {
                    countedInCombat = true;
                    activeEnemiesInCombat++;

                    if (activeEnemiesInCombat == 1)
                    {
                        AudioManager.Instance.PlayCombatMusic();
                    }
                }
            }
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
        // Remove from combat if it was EVER counted
        if (countedInCombat)
        {
            activeEnemiesInCombat--;
        }

        // Safety clamp
        activeEnemiesInCombat = Mathf.Max(0, activeEnemiesInCombat);

        // If no enemies left -> return to gameplay music
        if (activeEnemiesInCombat == 0)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
        Destroy(gameObject);
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = originalColor;
    }
}
