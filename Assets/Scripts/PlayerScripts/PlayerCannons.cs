using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class PlayerCannons : MonoBehaviour
{
    private const string CannonDamageKey = "PlayerCannonDamage";

    public Transform leftCannonPoint;
    public Transform rightCannonPoint;

    [SerializeField] private ParticleSystem leftFX;
    [SerializeField] private ParticleSystem rightFX;

    [Header("Defualt Values")]
    public float force = 8f;
    public float defaultCoolDown = 0.4f;
    public int defaultDamage = 10;
    // Current values (can be boosted)
    private float currentCoolDown;
    public int currentDamage;
    private float currentForce;

    private float lastLeftFireTime;
    private float lastRightFireTime;
    private float lastBarrelTime;

    [Header("Weapon Boost")]
    private float weaponBoostEndTime = 0f;
    private bool isWeaponBoosted = false;
    [SerializeField] private GameObject normalCannonBall;
    [SerializeField] private GameObject boostedCannonBall;

    private GameObject currentCannonBall;

    [Header("Visual FX")]
    public GameObject leftCannonBoostEffect;
    public GameObject rightCannonBoostEffect;
    public SpriteRenderer leftCannonSprite;
    public SpriteRenderer rightCannonSprite;
    private Color originalCannonColor;
    [SerializeField] private Image barrelCooldownButton;
    [SerializeField] private Image leftCannonCooldownImage;
    [SerializeField] private Image rightCannonCooldownImage;

    public GameObject explosiveBarrelPrefab;
    public Transform barrelSpawnPoint;
    public float barrelCooldown = 5f;

    

    void Start()
    {
        // Initialize with default values
        currentCoolDown = defaultCoolDown;
        currentDamage = PlayerPrefs.GetInt(CannonDamageKey, defaultDamage); //Load Player's current cannon damae
        currentForce = force;
        currentCannonBall = normalCannonBall;

        // Store original cannon color if we have sprites
        if (leftCannonSprite != null)
        {
            originalCannonColor = leftCannonSprite.color;
        }
    }

    void Update()
    {
        // Check if weapon boost has expired
        if (isWeaponBoosted && Time.time > weaponBoostEndTime)
        {
            EndWeaponBoost();
        }

        UpdateBarrelCooldownUI();
        UpdateCannonCooldownUI();
    }

    public void FireLeft()
    {
        if(Time.time > lastLeftFireTime + currentCoolDown)
        {
            lastLeftFireTime = Time.time;
            Fire(leftCannonPoint);
            leftFX.Play();
        }
    }

    public void FireRight()
    {
        if(Time.time > lastRightFireTime + currentCoolDown)
        {
            lastRightFireTime = Time.time;
            Fire(rightCannonPoint);
            rightFX.Play();
        }
    }

    void Fire(Transform firePoint)
    {

        GameObject ball = Instantiate(currentCannonBall, firePoint.position, firePoint.rotation);
        AudioManager.Instance.PlayCannon();

        // Apply force to cannon ball
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * currentForce, ForceMode2D.Impulse);

        // OPTIONAL: If you add a CannonBall script with damage variable
        PlayerCannonball cannonBallScript = ball.GetComponent<PlayerCannonball>();
        if (cannonBallScript != null)
        {
            cannonBallScript.damage = currentDamage;
        }
    }

    public void DeployBarrel()
    {
        if (Time.time < lastBarrelTime + barrelCooldown)
            return;

        lastBarrelTime = Time.time;

        GameObject barrel = Instantiate(explosiveBarrelPrefab, barrelSpawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = barrel.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            //Small push backwards when spawned
            Vector2 throwDirection = -transform.up;
            rb.AddForce(throwDirection * 4f, ForceMode2D.Impulse);
        }
    }

    private void UpdateCannonCooldownUI()
    {
        if(leftCannonCooldownImage != null)
        {
            float elapsedLeft = Time.time - lastLeftFireTime;
            float fillLeft = Mathf.Clamp01(elapsedLeft / currentCoolDown);
            leftCannonCooldownImage.fillAmount = fillLeft;
        }
        if(rightCannonCooldownImage != null)
        {
            float elapsedRight = Time.time - lastRightFireTime;
            float fillRight = Mathf.Clamp01(elapsedRight / currentCoolDown);
            rightCannonCooldownImage.fillAmount = fillRight;
        }
    }

    private void UpdateBarrelCooldownUI()
    {
        if (barrelCooldownButton == null)
            return;

        float elapsed = Time.time - lastBarrelTime;

        float fillPercent = Mathf.Clamp01(elapsed / barrelCooldown);

        barrelCooldownButton.fillAmount = fillPercent;
    }

    // ========== WEAPON BOOST METHODS ==========
    public void ApplyWeaponBoost(float fireRateMultiplier, float damageMultiplier, float duration)
    {
        currentCannonBall = boostedCannonBall;

        // Apply fire rate boost (lower cooldown = faster shooting)
        currentCoolDown = defaultCoolDown / fireRateMultiplier;

        // Apply damage boost
        currentDamage = Mathf.RoundToInt(defaultDamage * damageMultiplier);

        // Optional: Increase force too
        currentForce = force * 1.2f;

        // Set boost end time
        weaponBoostEndTime = Time.time + duration;
        isWeaponBoosted = true;

        // Visual feedback
        if (leftCannonBoostEffect != null) leftCannonBoostEffect.SetActive(true);
        if (rightCannonBoostEffect != null) rightCannonBoostEffect.SetActive(true);

        if (leftCannonSprite != null)
        {
            leftCannonSprite.color = Color.red;
            rightCannonSprite.color = Color.red;
        }
    }

    void EndWeaponBoost()
    {
        // Reset to default values
        currentCoolDown = defaultCoolDown;
        currentDamage = defaultDamage;
        currentForce = force;
        isWeaponBoosted = false;
        currentCannonBall = normalCannonBall;

        Debug.Log("Weapon boost ended");

        // Remove visual feedback
        if (leftCannonBoostEffect != null) leftCannonBoostEffect.SetActive(false);
        if (rightCannonBoostEffect != null) rightCannonBoostEffect.SetActive(false);

        if (leftCannonSprite != null)
        {
            leftCannonSprite.color = originalCannonColor;
            rightCannonSprite.color = originalCannonColor;
        }
    }

    // For testing without power-up
    void OnTestInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ApplyWeaponBoost(2f, 2f, 5f);
        }
    }

    public void IncreasePermanentDamage(int amount)
    {
        defaultDamage += amount;
        currentDamage = defaultDamage;

        Debug.Log("Cannon's Damage Increased t: " + defaultDamage);
    }
}