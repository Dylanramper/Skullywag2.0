using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    [Header("Shield Settings")]
    public float shieldDuration = 5f;  // How long shield lasts
    public int defenseBoost = 50;      // How much defense it adds
    public float rotationSpeed = 90f;  // Visual rotation
    public float floatSpeed = 2f;      // Up/down floating
    public float floatHeight = 0.3f;   // How high it floats

    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private Vector3 startPosition;

    void Start()
    {
        // Remember starting position for floating animation
        startPosition = transform.position;

        // Destroy after 10 seconds if not picked up
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        // Make it rotate slowly
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Make it float up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
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

<<<<<<< HEAD
            // Show UI icon (requires PowerUpUIManager in scene)
            PowerUpUIManager uiManager = FindAnyObjectByType<PowerUpUIManager>();
            if (uiManager != null && iconSprite != null)
            {
                uiManager.ShowPowerUpIcon(iconSprite);
            }

            PowerUpTimerUI timer = FindAnyObjectByType<PowerUpTimerUI>();
            if (timer != null)
            {
                timer.StartTimer(powerUpDuration);
            }

=======
>>>>>>> parent of 4839b2b (UI Power Up Icon Implemented)
            // Destroy the power-up
            Destroy(gameObject);
        }
    }
}