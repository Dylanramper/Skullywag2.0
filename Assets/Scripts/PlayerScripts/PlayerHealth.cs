using NUnit.Framework;
using System.Collections;
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
    [SerializeField] private SpriteRenderer playerSprite;
    private Color originalPlayerColor;


    [Header("UI Elements")]
    public Image shieldBar;
    public Text shieldText;
    public Image fillImage;
    [SerializeField] private ParticleSystem smokeFX1;
    [SerializeField] private ParticleSystem smokeFX2;

    [SerializeField] private GameOverScroll gameOverScroll;

    private float fillSmoothSpeed = 5;
    private Color originalColor;

    private float burnTimer;
    private float burnDPS;
    private float burnDamageAccumulator;

    void Start()
    {
        originalPlayerColor = playerSprite.color;
        originalColor = fillImage.color;
        currentHealth = maxHealth;

        if(fillImage != null)
        {
            fillImage.fillAmount = 1f;
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

        if(burnTimer > 0)
        {
            burnDamageAccumulator += burnDPS * Time.deltaTime;

            if(burnDamageAccumulator >= 1f)
            {
                int damageToApply = Mathf.FloorToInt(burnDamageAccumulator);
                burnDamageAccumulator -= damageToApply;

                TakeDamage(damageToApply);

                StartCoroutine(BurnFlash());
            }
            burnTimer -= Time.deltaTime;
        }

        // Smooth health bar animation
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, (float)currentHealth / maxHealth, fillSmoothSpeed * Time.deltaTime);
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
        AudioManager.Instance.PlayHit();
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        StartCoroutine(HealthFlash());

        //Play smoke FX when health is below 25%
        if(currentHealth <= 25)
        {
            smokeFX1.Play();
            smokeFX2.Play();
        }
        else if(currentHealth >= 26)
        {
            smokeFX1.Stop();
            smokeFX2.Stop();
        }


        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyBurn(float dps, float duration)
    {
        burnDPS = Mathf.Max(burnDPS, dps);
        burnTimer = Mathf.Max(burnTimer, duration);
    }

    private IEnumerator BurnFlash()
    {
        playerSprite.color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(0.5f);
        playerSprite.color = originalPlayerColor;
    }

    private IEnumerator HealthFlash()
    {
        fillImage.color = new Color(1f, 0.6f, 0.6f);
        yield return new WaitForSeconds(0.1f);
        fillImage.color = originalColor;
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

        if (playerSprite != null)
        {
            playerSprite.color = new Color(0.5f, 0.8f, 1f); // light blue tint
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

        if (playerSprite != null)
        {
            playerSprite.color = originalPlayerColor;
        }

        Debug.Log("Shield deactivated");
    }

    void Die()
    {
        Debug.Log("Player died!");
        Time.timeScale = 0f;
        gameOverScroll.ShowGameOver();
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