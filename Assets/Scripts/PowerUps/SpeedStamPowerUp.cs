using UnityEngine;
using System.Collections;

public class SpeedStamPowerUp : MonoBehaviour
{
    public PlayerMovement playerMove;
    [Header("Boost Settings")]
    public float speedMultiplier = 1.5f; // e.g., 1.5 = 50% faster
    public float boostDuration = 3f;      // How long the boost lasts

    [Header("Visuals")]
    public float rotationSpeed = 120f; // Faster rotation for speed feel
    public float floatSpeed = 3f;
    public float floatHeight = 0.3f;

    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupEffect; // Reuse your ShieldPickupEffect prefab here!

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, 10f); // Auto-destroy if not picked up
    }

    void Update()
    {
        // Float
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to get the player's movement script
            playerMove = other.GetComponent<PlayerMovement>();

            if (playerMove != null)
            {
                // Apply the speed boost to the player (instance method)
                playerMove.ApplySpeedBoost(speedMultiplier, boostDuration);
                playerMove.AddStamina(20f);
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

            // Destroy the power-up object
            Destroy(gameObject);
        }
    }
}