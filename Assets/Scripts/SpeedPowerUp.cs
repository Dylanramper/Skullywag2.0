using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    [Header("Boost Settings")]
    public float speedMultiplier = 1.5f;
    public float boostDuration = 3f;

    [Header("Visuals")]
    public float rotationSpeed = 120f;
    public float floatSpeed = 3f;
    public float floatHeight = 0.3f;

    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Method 1: Try to find a Rigidbody2D and boost it directly
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(ApplySpeedBoostToRigidbody(rb));
            }
            // Method 2: Try common movement script names
            else
            {
                ApplySpeedBoostToPlayer(other.gameObject);
            }

            // Effects
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

    // Method 1: Direct Rigidbody2D boost (works if player uses physics)
    System.Collections.IEnumerator ApplySpeedBoostToRigidbody(Rigidbody2D rb)
    {
        float originalSpeed = rb.linearVelocity.magnitude;
        rb.linearVelocity *= speedMultiplier;

        yield return new WaitForSeconds(boostDuration);

        // Note: This is simplistic - better to store original velocity direction
        rb.linearVelocity /= speedMultiplier;
    }

    // Method 2: Try different component names
    void ApplySpeedBoostToPlayer(GameObject player)
    {
        // Try multiple common script names
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            System.Type type = script.GetType();

            // Try to call ApplySpeedBoost if it exists
            System.Reflection.MethodInfo method = type.GetMethod("ApplySpeedBoost");
            if (method != null)
            {
                method.Invoke(script, new object[] { speedMultiplier, boostDuration });
                return;
            }

            // Try to find and modify a speed property
            System.Reflection.FieldInfo speedField = type.GetField("speed");
            if (speedField != null && speedField.FieldType == typeof(float))
            {
                float originalSpeed = (float)speedField.GetValue(script);
                speedField.SetValue(script, originalSpeed * speedMultiplier);

                // Start coroutine to revert speed
                script.StartCoroutine(RevertSpeedAfterDelay(script, speedField, originalSpeed, boostDuration));
                return;
            }
        }

        Debug.LogWarning("Could not find a way to apply speed boost to player!");
    }

    System.Collections.IEnumerator RevertSpeedAfterDelay(MonoBehaviour script, System.Reflection.FieldInfo speedField, float originalSpeed, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (script != null && speedField != null)
        {
            speedField.SetValue(script, originalSpeed);
            Debug.Log("Speed reverted to normal");
        }
    }
}