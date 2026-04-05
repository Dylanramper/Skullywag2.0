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
    public ParticleSystem particleFX;

    private Rigidbody2D rb;
    private bool isFloating = false;
    private float startFloatingAt;

    protected virtual void Start()
    {
        startFloatingAt = Random.Range(0.7f, 1.2f);
        particleFX = GetComponentInChildren<ParticleSystem>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        // Start floating after a short delay
        Invoke(nameof(EnableFloating), startFloatingAt);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.CoinPickUp();
            particleFX.Play();
            Collect();
            Debug.Log("collected");
        }
    }

    void EnableFloating()
    {
        isFloating = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic; // stop physics
        }

        startPosition = transform.position;
    }

    protected virtual void Collect()
    {
        // Add score using ScoreManager
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