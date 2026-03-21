using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;

    [Header("Mortar")]
    public float mortarCooldown = 3f;
    public float cannonCooldown = 2f;
    public float barrelCooldown = 4f;

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

    private float mortarTimer;
    private float cannonTimer;
    private float barrelTimer;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
        HandleCooldowns();
        DecideAttack();
    }

    void UpdatePhase()
    {
        float healthPercent = health / maxHealth;

        if (healthPercent <= 0.3f)
        {
            // Phase 3
        }
        else if (healthPercent <= 0.7f)
        {
            // Phase 2
        }
        else
        {
            // Phase 1
        }
    }

    void DecideAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

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
        else if (cannonTimer <= 0f)
        {
            SideCannons();
            cannonTimer = cannonCooldown;
        }
    }

    void FireCannons(Transform cannonA, Transform cannonB)
    {
        if (cannonballPrefab == null)
        {
            Debug.LogError("Cannonball Prefab not assigned!");
            return;
        }

        AudioManager.Instance.PlayCannon();

        FireSingleCannon(cannonA);
        FireSingleCannon(cannonB);
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
           Vector2 targetPos = player.position;

           GameObject indicator = Instantiate(mortarIndicatorPrefab, targetPos, Quaternion.identity);

           Destroy(indicator, mortarDelay);

           StartCoroutine(MortarExplosion(targetPos));
    }

    IEnumerator MortarExplosion(Vector2 position)
    {
        yield return new WaitForSeconds(mortarDelay);

        AudioManager.Instance.PlayExplosion();
        AudioManager.Instance.PlayHit();

        Instantiate(explosionPrefab, position, Quaternion.identity);
    }

    void DropBarrel()
    {
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

    void HandleCooldowns()
    {
        mortarTimer -= Time.deltaTime;
        cannonTimer -= Time.deltaTime;
        barrelTimer -= Time.deltaTime;
    }

    bool IsPlayerBehind()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;

        float dot = Vector2.Dot(transform.up, toPlayer);

        // If dot is negative, player is behind
        return dot < -0.3f;
    }
}
