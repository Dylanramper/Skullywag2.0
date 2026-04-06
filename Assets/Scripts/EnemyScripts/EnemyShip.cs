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

    public static int activeEnemiesInCombat;
    private bool countedInCombat = false;

    [Header("Drops")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 1;
    [SerializeField] private int maxCoins = 3;

    [Header("Power-Up Drops")]
    [SerializeField, Range(0f, 1f)] private float powerUpDropChance = 0.8f;

    [SerializeField] private float minDropForce = 2f;
    [SerializeField] private float maxDropForce = 4f;

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
            //Disengage if too far
            if (distance > loseAggroRange)
            {
                playerDetected = false;

                if (countedInCombat)
                {
                    countedInCombat = false;
                    activeEnemiesInCombat--;
                }
            }
        }
        else if(distance < detectionRange)
        {
            //If not Aggro'd and player is in detection range. Detected set to true.
                playerDetected = true;

                if (!countedInCombat)
{
                    countedInCombat = true;
                    activeEnemiesInCombat++;

                    if (!AudioManager.Instance.bossActive && activeEnemiesInCombat >= 1)
                    {
                        AudioManager.Instance.PlayCombatMusic();
                    }
                }
            }
        }

    protected virtual void Wander()
    {
        //Always move forward
        rb.linearVelocity = transform.up * moveSpeed * 0.5f;

        Vector2 pos = transform.position;

        //Occasionally turn
        if (Time.time > nextWanderChangeTime)
        {
            int choice = Random.Range(0, 3); //0 = left : 2 = right
            wanderTurnDirection = choice == 0 ? -1 : choice == 2 ? 1f : 0f;
            nextWanderChangeTime = Time.time + wanderChangeInterval;
        }

        //Apply turn
        rb.MoveRotation(rb.rotation + wanderTurnDirection * wanderTurnStrength * Time.fixedDeltaTime);

            if (MapBounds.Instance.IsNearEdge(pos, 1f))
            {
                Vector2 toCenter = (Vector2.zero - pos).normalized;

                // Get current forward direction
                Vector2 forward = transform.up;

            // Determine which way to turn (-1 = left, 1 = right)
            float turnDirection = Vector3.Cross(forward, toCenter).z;
            wanderTurnDirection = Mathf.Lerp(wanderTurnDirection, turnDirection, Time.deltaTime * 2f);
        }
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
        //Drop coins
        int coinCount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinCount; i++)
        {
            // Slight random offset so coins don't stack
            Vector3 spawnPos = transform.position + (Vector3)(Random.insideUnitCircle * 0.5f);
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            // Add push force
            Rigidbody2D rbCoin = coin.GetComponent<Rigidbody2D>();
            if (rbCoin != null)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector2 forceDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float force = Random.Range(minDropForce, maxDropForce);
                rbCoin.AddForce(forceDir * force, ForceMode2D.Impulse);
            }
        }

        // Drop Power-up
        if (Random.value < powerUpDropChance)
        {
            PowerUpManager.Instance.SpawnRandom(transform.position, true);
            Debug.Log("Power up dropped");
        }

        // Remove from combat if it was EVER counted
        if (countedInCombat)
        {
            activeEnemiesInCombat--;
        }

        // Safety clamp
        activeEnemiesInCombat = Mathf.Max(0, activeEnemiesInCombat);

        if (!AudioManager.Instance.bossActive && activeEnemiesInCombat == 0)
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
