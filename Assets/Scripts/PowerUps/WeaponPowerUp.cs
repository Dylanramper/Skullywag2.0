using UnityEngine;

public class WeaponPowerUp : MonoBehaviour
{
    [Header("Weapon Boost Settings")]
    [Tooltip("Fire rate multiplier (e.g., 2.0 = twice as fast)")]
    public float fireRateMultiplier = 2.0f;
    [Tooltip("Damage multiplier (e.g., 2.0 = double damage)")]
    public float damageMultiplier = 2.0f;
    [Tooltip("How long the boost lasts")]
    public float boostDuration = 5f;

    [Header("Visuals")]
    public float rotationSpeed = 150f;
    public float floatSpeed = 3f;
    public float floatHeight = 0.3f;
    public Color glowColor = Color.red;

    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = glowColor;
        }

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        // Float and rotate animation
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Pulsing glow effect
        float pulse = Mathf.Sin(Time.time * 3f) * 0.2f + 0.8f;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, glowColor, pulse);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // FIXED: Look for PlayerCannons instead of PlayerShooting
            PlayerCannons playerCannons = other.GetComponent<PlayerCannons>();

            if (playerCannons != null)
            {
                playerCannons.ApplyWeaponBoost(fireRateMultiplier, damageMultiplier, boostDuration);
            }

            // Visual/Audio feedback
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}