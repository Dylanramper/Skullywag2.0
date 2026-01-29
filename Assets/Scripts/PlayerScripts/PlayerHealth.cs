using UnityEngine;
using UnityEngine.UI; // For UI if you want

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Shield Settings")]
    public bool hasShield = false;
    public float shieldTimeLeft = 0f;
    public int shieldDefense = 0;
    public GameObject shieldVisual; // Optional: shield graphic on player

    [Header("UI Elements")]
    public Slider healthSlider;     // Optional
    public Image shieldBar;         // Optional
    public Text shieldText;         // Optional

    void Start()
    {
        currentHealth = maxHealth;

        // Update UI if set
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Hide shield visual at start
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    void Update()
    {
        // Count down shield timer
        if (hasShield)
        {
            shieldTimeLeft -= Time.deltaTime;

            // Update UI if set
            if (shieldBar != null)
            {
                shieldBar.fillAmount = shieldTimeLeft / 5f; // 5 is default duration
            }
            if (shieldText != null)
            {
                shieldText.text = "Shield: " + Mathf.CeilToInt(shieldTimeLeft).ToString();
            }

            // Remove shield when time runs out
            if (shieldTimeLeft <= 0)
            {
                DeactivateShield();
            }
        }
    }

    // Call this when player takes damage
    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            // Reduce damage with shield
            damage = Mathf.Max(1, damage - shieldDefense);
            Debug.Log("Shield absorbed some damage! Damage taken: " + damage);
        }

        currentHealth -= damage;

        // Update UI if set
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log("Player health: " + currentHealth + "/" + maxHealth);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Call this from ShieldPowerUp script
    public void ActivateShield(float duration, int defense)
    {
        hasShield = true;
        shieldTimeLeft = duration;
        shieldDefense = defense;

        // Show shield visual
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }

        Debug.Log("Shield activated! Defense +" + defense + " for " + duration + " seconds");
    }

    void DeactivateShield()
    {
        hasShield = false;
        shieldTimeLeft = 0f;
        shieldDefense = 0;

        // Hide shield visual
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }

        Debug.Log("Shield deactivated");
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add your death logic here (respawn, game over, etc.)
        // Time.timeScale = 0; // Pause game
    }

    // Optional: For testing with keyboard
    void OnTestInput()
    {
        // Press T to test taking damage
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(20);
        }

        // Press Y to test shield (for debugging)
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ActivateShield(5f, 30);
        }
    }
}