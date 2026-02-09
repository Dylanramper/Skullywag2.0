using UnityEngine;
using System.Collections;

public class PlayerCannons : MonoBehaviour
{
    public Transform leftCannonPoint;
    public Transform rightCannonPoint;
    public GameObject CannonBall;

    // Default values
    public float force = 8f;
    public float defaultCoolDown = 0.4f;
    public int defaultDamage = 1; // Add this if your cannon balls don't have damage yet

    // Current values (can be boosted)
    private float currentCoolDown;
    private int currentDamage;
    private float currentForce;

    private float lastFireTime;

    // Weapon boost tracking
    private float weaponBoostEndTime = 0f;
    private bool isWeaponBoosted = false;

    // Optional visual feedback
    public GameObject leftCannonBoostEffect;
    public GameObject rightCannonBoostEffect;
    public SpriteRenderer leftCannonSprite;
    public SpriteRenderer rightCannonSprite;
    private Color originalCannonColor;

    void Start()
    {
        // Initialize with default values
        currentCoolDown = defaultCoolDown;
        currentDamage = defaultDamage;
        currentForce = force;

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
    }

    public void FireLeft()
    {
        if (Time.time > lastFireTime + currentCoolDown)
        {
            Fire(leftCannonPoint, -transform.right);
        }
    }

    public void FireRight()
    {
        if (Time.time > lastFireTime + currentCoolDown)
        {
            Fire(rightCannonPoint, transform.right);
        }
    }

    void Fire(Transform firePoint, Vector2 direction)
    {
        lastFireTime = Time.time;

        GameObject ball = Instantiate(CannonBall, firePoint.position, Quaternion.identity);

        // Apply force to cannon ball
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * currentForce, ForceMode2D.Impulse);

        // OPTIONAL: If you add a CannonBall script with damage variable
        PlayerCannonball cannonBallScript = ball.GetComponent<PlayerCannonball>();
        if (cannonBallScript != null)
        {
            cannonBallScript.damage = currentDamage;
        }

        Debug.Log($"Fired! Damage: {currentDamage}, Cooldown: {currentCoolDown}s");
    }

    // ========== WEAPON BOOST METHODS ==========

    // Called by WeaponPowerUp script
    public void ApplyWeaponBoost(float fireRateMultiplier, float damageMultiplier, float duration)
    {
        // Apply fire rate boost (lower cooldown = faster shooting)
        currentCoolDown = defaultCoolDown / fireRateMultiplier;

        // Apply damage boost
        currentDamage = Mathf.RoundToInt(defaultDamage * damageMultiplier);

        // Optional: Increase force too
        currentForce = force * 1.2f;

        // Set boost end time
        weaponBoostEndTime = Time.time + duration;
        isWeaponBoosted = true;

        Debug.Log($"Weapon Boosted! Fire Rate: {fireRateMultiplier}x, Damage: {currentDamage}, Duration: {duration}s");

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

    public void testClick()
    {
        Debug.Log("Clicking");
    }
}