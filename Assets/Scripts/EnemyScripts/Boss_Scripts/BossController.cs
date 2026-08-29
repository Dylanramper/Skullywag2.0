using System.Collections;
using UnityEngine;

public class BossController : BaseBoss
{
    [Header("Base Stats")]
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

    [Header("VisualFX")]
    public ParticleSystem damageFX1;
    public ParticleSystem damageFX2;
    public ParticleSystem damageFX3;
    public ParticleSystem cannonRightFX1;
    public ParticleSystem cannonRightFX2;
    public ParticleSystem cannonLeftFX1;
    public ParticleSystem cannonLeftFX2;
    private bool damagedFX;

    public GameObject cannonballPrefab;

    [Header("Death VFX")]
    [SerializeField] private ParticleSystem explsion1;
    [SerializeField] private ParticleSystem explsion2;
    [SerializeField] private ParticleSystem explsion3;
    [SerializeField] private ParticleSystem explsion4;
    private Rigidbody2D rb;

    [SerializeField] private float explosionDelay = 0.5f;
    [SerializeField] private float secondExplosions = 0.4f;
    [SerializeField] private float thirdExplosions = 0.2f;

    [Header("Barrel")]
    public GameObject barrelPrefab;
    public Transform barrelDropPoint;
    public float barrelPushForce = 3f;

    [Header("Rowboats")]
    public GameObject rowboatPrefab;
    public int rowboatCount = 5;
    public float summonCooldown = 8f;

    public float moveSpeed = 1f;
    public float rotationSpeed = 0.2f;

    private float summonTimer;

    private float mortarTimer;
    private float cannonTimer;
    private float barrelTimer;

    private int broadsideSide = 1; // 1 = right, -1 = left
    private float broadsideSwitchCooldown = 2f;
    private float broadsideSwitchTimer;
    float broadsideLockTimer = 0f;
    float requiredLockTime = 0.1f;
    bool IsBroadsideLocked(float tolerance = 6f)
    {
        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        float targetAngle = 45f * broadsideSide;

        float angleDiff = Mathf.DeltaAngle(-angleToPlayer, targetAngle);

        return Mathf.Abs(angleDiff) < tolerance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start(); // sets health = maxHealth

        if (barrelDropPoint == null)
        {
            GameObject barrelPointObj = GameObject.Find("BarrelPoint"); // Name of your child object in hierarchy
            if (barrelPointObj != null)
            {
                barrelDropPoint = barrelPointObj.transform;
                Debug.Log("Auto-assigned barrelDropPoint to " + barrelPointObj.name);
            }
            else
            {
                Debug.LogWarning("Cannot find BarrelPoint in the scene!");
            }
        }

        if (mortarFirePoint == null) Debug.LogError("mortarFirePoint is NULL!");
        if (leftCannon1 == null || rightCannon1 == null) Debug.LogError("Cannon(s) missing!");

        AudioManager.Instance.bossActive = true;
        AudioManager.Instance.PlayCombatMusic();

        damagedFX = false;
        rb = GetComponent<Rigidbody2D>();

        if (bossHealthbar != null)
            bossHealthbar.UpdateHealth(health, maxHealth);

        BossIndicator indicator = FindFirstObjectByType<BossIndicator>();
        if (indicator != null)
        {
            indicator.SetBoss(this.transform);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); // IMPORTANT

        HandleCooldowns();
        DecideAttack();
        Move();

        broadsideSwitchTimer -= Time.deltaTime;

        if (broadsideSwitchTimer <= 0f)
        {
            ChooseBroadsideSide();
            broadsideSwitchTimer = broadsideSwitchCooldown;
        }

        if (IsBroadsideLocked())
            broadsideLockTimer += Time.deltaTime;
        else
            broadsideLockTimer = 0f;

        damagedFX = health <= 40f;
    }

    public void SetBossHealthbar(BossHPBar hpBar)
    {
        bossHealthbar = hpBar;

        // Immediately sync health
        if (bossHealthbar != null)
            bossHealthbar.UpdateHealth(health, maxHealth);
    }

    void Move()
    {
        Steer();
        // Always move forward
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    void Steer()
    {
        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);
        float turnDirection = Mathf.Sign(angleToPlayer);

        float desiredDistance = 10f;

        if (distance > desiredDistance)
        {
            // Chase player (like brig)
            transform.Rotate(0, 0, turnDirection * rotationSpeed * 100f * Time.deltaTime);
        }
        else
        {
            // Maintain broadside instead of orbiting blindly
            MaintainBroadside(angleToPlayer);
        }
    }

    void ChooseBroadsideSide()
    {
        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        float dot = Vector2.Dot(transform.right, toPlayer);

        broadsideSide = dot >= 0 ? 1 : -1;
    }

    void MaintainBroadside(float angleToPlayer)
    {
        float targetAngle = 45f * broadsideSide;

        float angleDiff = Mathf.DeltaAngle(-angleToPlayer, targetAngle);

        float turnStrength = Mathf.Clamp(angleDiff / 45f, -1f, 1f);

        transform.Rotate(0, 0, turnStrength * rotationSpeed * 120f * Time.deltaTime);
    }

    void DecideAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // PRIORITY 1: Cannons when broadside is locked
        if (cannonTimer <= 0f && broadsideLockTimer >= requiredLockTime)
        {
            SideCannons();
            cannonTimer = cannonCooldown;
            broadsideLockTimer = 0f;
            return;
        }

        // Priority 2: Mortar (long range)
        if (distance > 6f && mortarTimer <= 0f)
        {
            MortarAttack();
            mortarTimer = mortarCooldown;
            return;
        }

        // Priority 3: Barrel (close + behind)
        if (IsPlayerBehind() && barrelTimer <= 0f)
        {
            DropBarrel();
            barrelTimer = barrelCooldown;
            return;
        }
    }
    void FireSingleCannon(Transform cannon)
    {
        if (cannon == null || cannonballPrefab == null)
        {
            Debug.LogWarning("Cannot fire cannon: missing reference!");
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

    IEnumerator FireRightCannonsFX()
    {
        if (cannonRightFX1 != null) cannonRightFX1.Play();

        yield return new WaitForSeconds(0.5f);

        if (cannonRightFX2 != null) cannonRightFX2.Play();

        yield return new WaitForSeconds(0.5f);

        if (cannonRightFX1 != null) cannonRightFX1.Play();
    }

    IEnumerator FireLeftCannonsFX()
    {
        if (cannonLeftFX1 != null) cannonLeftFX1.Play();

        yield return new WaitForSeconds(0.5f);

        if (cannonLeftFX2 != null) cannonLeftFX2.Play();

        yield return new WaitForSeconds(0.5f);

        if (cannonLeftFX1 != null) cannonLeftFX1.Play();
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
        // Auto-assign barrelDropPoint if null
        if (barrelDropPoint == null)
        {
         // First try as a child of this boss
         Transform child = transform.Find("BarrelPoint");
         if (child != null)
         {
              barrelDropPoint = child;
              Debug.Log("Auto-assigned barrelDropPoint as child: " + child.name);
         }
         else
         {
              // Fallback: search in the scene
              GameObject obj = GameObject.Find("BarrelPoint");
            if (obj != null)
            {
                barrelDropPoint = obj.transform;
                Debug.Log("Auto-assigned barrelDropPoint in scene: " + obj.name);
            }
            else
            {
                Debug.LogWarning("Cannot drop barrel: BarrelPoint not found!");
                return; // exit early since we can't drop
            }
        }
    }

    if (barrelPrefab == null)
    {
        Debug.LogWarning("Cannot drop barrel: barrelPrefab missing!");
        return;
    }

    // Instantiate and push
    GameObject barrelInstance = Instantiate(barrelPrefab, barrelDropPoint.position, Quaternion.identity);

    Rigidbody2D rb = barrelInstance.GetComponent<Rigidbody2D>();
    if (rb != null)
        rb.AddForce(-transform.up * barrelPushForce, ForceMode2D.Impulse);
    else
        Debug.LogWarning("Barrel has no Rigidbody2D!");
}

    void SideCannons()
    {
        if (broadsideSide == 1)
        {
            StartCoroutine(FireCannonsDelayed(rightCannon1, rightCannon2));
            StartCoroutine(FireRightCannonsFX());
        }
        else if (broadsideSide == -1)
        {
            StartCoroutine(FireCannonsDelayed(leftCannon1, leftCannon2));
            StartCoroutine(FireLeftCannonsFX());
        }
    }

    public void SetMortarIndicator(GameObject indicatorPrefab)
    {
        mortarIndicatorPrefab = indicatorPrefab;
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

    IEnumerator SummonRowboatsSequence()
    {
        // Play fog horn
        AudioManager.Instance.PlayFogHorn();

        // Big camera shake
        GameManager.Instance.ShakeCamera(3f, 0.8f);

        // Small delay for anticipation
        yield return new WaitForSeconds(1.2f);

        SummonRowboats();
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // handles health + death

        if (bossHealthbar != null)
            bossHealthbar.UpdateHealth(health, maxHealth);

        AudioManager.Instance.PlayHit();
        GameManager.Instance.ShakeCamera(0.1f, 0.08f);
    }

    protected override void Die()
    {
        AudioManager.Instance.bossActive = false;
        EnemyShip.activeEnemiesInCombat--;
        BossIndicator indicator = FindFirstObjectByType<BossIndicator>();
        if (indicator != null)
        {
            indicator.ClearBoss();
        }

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        //Stop moving
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        //Disable Collider
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col != null)
            col.enabled = false;

        //First Explosion
        if (explsion1 != null)
        {
            explsion1.transform.parent = null;
            explsion1.Play();
            AudioManager.Instance.PlayExplosion();

            float totalDuration1 = explsion1.main.duration + explsion1.main.startLifetime.constantMax;

            Destroy(explsion1.gameObject, totalDuration1);
        }

        yield return new WaitForSeconds(explosionDelay);

        //Second Explosion
        if (explsion2 != null)
        {
            explsion2.transform.parent = null;
            explsion2.Play();
            AudioManager.Instance.PlayExplosion();

            float totalDuration2 = explsion2.main.duration + explsion2.main.startLifetime.constantMax;

            Destroy(explsion2.gameObject, totalDuration2);
        }

        yield return new WaitForSeconds(secondExplosions);
        //Third Explosion
        if (explsion3 != null)
        {
            explsion3.transform.parent = null;
            explsion3.Play();
            AudioManager.Instance.PlayExplosion();

            float totalDuration3 = explsion3.main.duration + explsion3.main.startLifetime.constantMax;

            Destroy(explsion3.gameObject, totalDuration3);
        }
        yield return new WaitForSeconds(thirdExplosions);

        //Forth Explosion
        if (explsion4 != null)
        {
            explsion4.transform.parent = null;
            explsion4.Play();
            AudioManager.Instance.PlayExplosion();

            float totalDuration4 = explsion4.main.duration + explsion4.main.startLifetime.constantMax;

            Destroy(explsion4.gameObject, totalDuration4);
        }
        Destroy(gameObject);
    }
}
