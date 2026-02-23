using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Collectable Settings")]
    public int scoreValue = 5;
    public AudioClip pickupSound;
    public GameObject pickupEffect;
    public float rotationSpeed = 180f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;

    private Vector3 startPosition;
    public ParticleSystem particleSystem;

    protected virtual void Start()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        startPosition = transform.position;
        Destroy(gameObject, 15f); // Auto-destroy after 15 seconds
    }

    void Update()
    {
        // Make it spin
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Make it float up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            particleSystem.Play();
            Collect();
        }
    }

    protected virtual void Collect()
    {
        // Add score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }
        else
        {
            Debug.Log($"Collected! +{scoreValue} points");
        }

        // Play sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Spawn effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}