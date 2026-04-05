using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    [Header("Shield Settings")]
    public float shieldDuration = 5f;  // How long shield lasts
    public int defenseBoost = 50;      // How much defense it adds
    public float rotationSpeed = 90f;  // Visual rotation
    public float floatSpeed = 2f;      // Up/down floating
    public float floatHeight = 0.3f;   // How high it floats
    public float powerUpDuration = 5f;


    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    [Header("UI")]
    public Sprite iconSprite;           // Drag the shield icon here
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private bool isFloating = false;
    private float startFloatingAt;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startFloatingAt = Random.Range(0.7f, 1.2f);

        // Try to grab sprite
        if (iconSprite == null)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                iconSprite = sr.sprite;
        }

        startPosition = transform.position;

        Invoke(nameof(EnableFloating), startFloatingAt);

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (isFloating)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void EnableFloating()
    {
        isFloating = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        startPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player touched the power-up
        if (other.CompareTag("Player"))
        {
            // Try to give shield to player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ActivateShield(shieldDuration, defenseBoost);
            }

            // Play sound if set
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Create effect if set
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Show UI icon (requires PowerUpUIManager in scene)
            PowerUpUIManager uiManager = FindAnyObjectByType<PowerUpUIManager>();
            if (uiManager != null && iconSprite != null)
            {
                uiManager.ShowPowerUpIcon(iconSprite);
            }

            // Destroy the power-up
            Destroy(gameObject);
        }
    }
}