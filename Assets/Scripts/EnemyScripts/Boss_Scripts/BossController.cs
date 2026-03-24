using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public Transform player;

    [Header("Base Stats")]
    [SerializeField] FloatingHealthbar healthbar;
    [SerializeField] BossHPBar bossHealthbar;

    [Header("Mortar")]
    public GameObject mortarProjectilePrefab;
    public Transform mortarFirePoint;
    public float mortarCooldown = 3f;
    public float cannonCooldown = 2f;
    public float barrelCooldown = 4f;

    public ParticleSystem mortarParticles;
    public GameObject mortarIndicatorPrefab;
    public float mortarDelay = 1f;
    public GameObject explosionPrefab;

    [Header("Side Cannons")]
    public Transform leftCannon1;
    public Transform leftCannon2;

    public Transform rightCannon1;
    public Transform rightCannon2;

    public ParticleSystem cannonFX1;
    public ParticleSystem cannonFX2;

    public GameObject cannonballPrefab;

    [Header("Barrel")]
    public GameObject barrelPrefab;
    public Transform barrelDropPoint;
    public float barrelPushForce = 3f;

    [Header("Rowboats")]
    public GameObject rowboatPrefab;
    public int rowboatCount = 5;
    public float summonCooldown = 8f;

    public float moveSpeed = 1f;
    public float rotationSpeed = 1f;

    private float summonTimer;

    private float mortarTimer;
    private float cannonTimer;
    private float barrelTimer;
    private int orbitDirection = 1;

    private int currentPhase = 1;

    private float health;
    private float maxHealth = 100;

    int GetPlayerSide()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;

        float dot = Vector2.Dot(transform.right, toPlayer);

        if (dot > 0.3f)
            return 1; // Right side
        else if (dot < -0.3f)
            return -1; // Left side
        else
            return 0; // Front or back
    }

    bool IsFacingPlayer(float threshold = 0.8f)
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;
        float dot = Vector2.Dot(transform.up, toPlayer);
        return dot > threshold;
    }

    bool IsBroadsideAligned(float threshold = 0.7f)
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;

        float dot = Vector2.Dot(transform.right, toPlayer);

        return Mathf.Abs(dot) > threshold;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        mortarParticles = GetComponentInChildren<ParticleSystem>();
        healthbar = GetComponentInChildren<FloatingHealthbar>();
        healthbar.UpdateHealthbar(health, maxHealth);

        bossHealthbar.UpdateHealth(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
        HandleCooldowns();
        DecideAttack();
        Move();
    }

    void UpdatePhase()
    {
        float healthPercent = health / maxHealth;

        if (healthPercent <= 0.3f)
        {
            currentPhase = 3;
        }
        else if (healthPercent <= 0.7f)
        {
            currentPhase = 2;
        }
        else
        {
            currentPhase = 1;
        }
    }

    void Move()
    {
        Steer();
        // Always move forward
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    void Steer()
    {
        Vector2 toPlayer = (player.position - transform.position);
        float distance = toPlayer.magnitude;

        Vector2 dirToPlayer = toPlayer.normalized;

        float desiredDistance = 7f;

        Vector2 desiredDirection;

        if (distance > desiredDistance + 1f)
        {
            // Chase player
            desiredDirection = dirToPlayer;
        }
        else if (distance < desiredDistance - 1f)
        {
            // Back away
            desiredDirection = -dirToPlayer;
        }
        else
        {
            // Orbit (THIS is the magic)
            desiredDirection = new Vector2(-dirToPlayer.y, dirToPlayer.x) * orbitDirection;
        }

        // Steering using cross product
        float cross = Vector3.Cross(transform.up, desiredDirection).z;

        float turn = Mathf.Clamp(cross, -1f, 1f);

        transform.Rotate(0, 0, -turn * rotationSpeed * 200f * Time.deltaTime);
    }

    void DecideAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Phase 3 special ability (highest priority)
        if (currentPhase == 3 && summonTimer <= 0f)
        {
            SummonRowboats();
            summonTimer = summonCooldown;
            return;
        }

        if (distance > 6f && mortarTimer <= 0f)
        {
            MortarAttack();
            mortarTimer = mortarCooldown;
        }
        else if (IsPlayerBehind() && barrelTimer <= 0f)
        {
            DropBarrel();
            barrelTimer = barrelCooldown;
        }
        else if (cannonTimer <= 0f && IsBroadsideAligned())
        {
            SideCannons();
            cannonTimer = cannonCooldown;
        }
    }
    void FireSingleCannon(Transform cannon)
    {
        if (cannon == null)
        {
            Debug.LogWarning("Cannon transform missing!");
            return;
        }

        GameObject cannonball = Instantiate(cannonballPrefab, cannon.position, cannon.rotation);

        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        rb.linearVelocity = -cannon.transform.right * 10f;
    }

    IEnumerator FireCannonsDelayed(Transform cannonA, Transform cannonB)
    {
        AudioManager.Instance.PlayCannon();

        FireSingleCannon(cannonA);

        AudioManager.Instance.PlayCannon();
        yield return new WaitForSeconds(0.5f);

        FireSingleCannon(cannonB);
        AudioManager.Instance.PlayCannon();
        yield return new WaitForSeconds(0.5f);

        FireSingleCannon(cannonA);
        AudioManager.Instance.PlayCannon();
    }

    void MortarAttack()
    {
        if (currentPhase == 3)
        {
            StartCoroutine(MortarBurst());
        }
        else
        {
            FireSingleMortar(player.position);
        }
    }
    void FireSingleMortar(Vector2 targetPos)
    {
        GameObject indicator = Instantiate(mortarIndicatorPrefab, targetPos, Quaternion.identity);
        Destroy(indicator, mortarDelay);
        GameObject proj = Instantiate(mortarProjectilePrefab, mortarFirePoint.position, Quaternion.identity);

        MortarProjectile mortar = proj.GetComponent<MortarProjectile>();
        mortar.Launch(mortarFirePoint.position, targetPos);
        mortarParticles.Play();
    }

    IEnumerator MortarBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
            Vector2 targetPos = (Vector2)player.position + randomOffset;

            FireSingleMortar(targetPos);

            yield return new WaitForSeconds(0.3f);
        }
    }

    void DropBarrel()
    {
        if (barrelPrefab == null || barrelDropPoint == null)
        {
            Debug.LogError("Barrel setup missing!");
            return;
        }

        GameObject barrelInstance = Instantiate(barrelPrefab, barrelDropPoint.position, Quaternion.identity);

        Rigidbody2D rb = barrelInstance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(-transform.up * barrelPushForce, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning("Barrel has no Rigidbody2D!");
        }

        Debug.Log("Barrel Dropped");
    }

    void SideCannons()
    {
        int side = GetPlayerSide();

        if (side == 1)
        {
            StartCoroutine(FireCannonsDelayed(rightCannon1, rightCannon2));
        }
        else if (side == -1)
        {
            StartCoroutine(FireCannonsDelayed(leftCannon1, leftCannon2));
        }
    }

    void SummonRowboats()
    {
        if (rowboatPrefab == null || player == null)
        {
            Debug.LogError("Rowboat prefab or player missing!");
            return;
        }

        float spawnRadius = 15f; // distance from player

        for (int i = 0; i < rowboatCount; i++)
        {
            // Random position around player
            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector2 spawnPos = (Vector2)player.position + offset;

            // Direction toward player
            Vector2 direction = (player.position - (Vector3)spawnPos).normalized;

            // Convert direction to rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Adjust depending on your sprite orientation
            Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);

            Instantiate(rowboatPrefab, spawnPos, rotation);
        }
    }

    void HandleCooldowns()
    {
        mortarTimer -= Time.deltaTime;
        cannonTimer -= Time.deltaTime;
        barrelTimer -= Time.deltaTime;
        summonTimer -= Time.deltaTime;
    }

    bool IsPlayerBehind()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;

        float dot = Vector2.Dot(transform.up, toPlayer);

        // If dot is negative, player is behind
        return dot < -0.3f;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        bossHealthbar.UpdateHealth(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
        healthbar.UpdateHealthbar(health, maxHealth);
        AudioManager.Instance.PlayHit();
        GameManager.Instance.ShakeCamera(0.1f, 0.08f);
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");

        // TODO: add explosion, loot, etc.
        Destroy(gameObject);
    }
}
