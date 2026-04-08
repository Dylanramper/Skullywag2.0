using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 15f;
    [SerializeField] private float staminaRegenRate = 15f;
    [SerializeField] private UnityEngine.UI.Image staminaFillImage;

    private Color originalColor;
    private float currentStamina = 100;
    private bool isBoosting;

    [Header("Trail")]
    [SerializeField] private TrailRenderer trail;

    private AnimationCurve normalWidthCurve;
    private AnimationCurve boostedWidthCurve;

    [Header("Speed Power-up")]
    private float speedPowerUpMultiplier = 1f;
    private float speedBoostEndTime = 0f;
    private bool isSpeedBoostActive = false;

    private Rigidbody2D rb;
    public float speed;
    public float turnSpeed;
    private int turnDir = 0;

    [SerializeField] private ControlMode controlMode = ControlMode.Buttons;
    [SerializeField] private Joystick joystick;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
        originalColor = staminaFillImage.color;

        //Load Save data(Previously used control scheme)
        if (PlayerPrefs.HasKey("ControlMode"))
        {
            controlMode = (ControlMode)PlayerPrefs.GetInt("ControlMode");
        }

        if (trail != null)
        {
            // Store original width curve
            normalWidthCurve = trail.widthCurve;

            // Create a boosted curve (thicker at the end)
            boostedWidthCurve = new AnimationCurve();
            boostedWidthCurve.AddKey(0f, 0.47f);  // start width
            boostedWidthCurve.AddKey(1f, 1f);  // end width (thicker)
        }
    }

    private void FixedUpdate()
    {
        HandleBoost();
        HandleSpeedPowerUp();

        float finalSpeed = speed;

        // Apply stamina boost
        if (isBoosting && currentStamina > 0f)
        {
            finalSpeed *= boostMultiplier;
        }

        // Apply power-up boost
        finalSpeed *= speedPowerUpMultiplier;

        // Move player
        rb.linearVelocity = (Vector2)transform.up * finalSpeed;

        HandleRotation();
        HandleMapBoundary();
    }

    private void HandleSpeedPowerUp()
    {
        if (isSpeedBoostActive && Time.time > speedBoostEndTime)
        {
            speedPowerUpMultiplier = 1f;
            isSpeedBoostActive = false;

            // Reset trail
            if (trail != null)
                trail.widthCurve = normalWidthCurve;
        }
    }

    private void HandleMapBoundary()
    {
        Vector2 playerPos = transform.position;

        if (MapBounds.Instance.IsNearEdge(playerPos, 0f)) // buffer optional
        {
            // Direction from player to center
            Vector2 pushDir = MapBounds.Instance.GetDirectionToCenter(playerPos);

            // Optional: add a little forward blending
            Vector2 forward = transform.up;
            Vector2 finalDir = Vector2.Lerp(pushDir, forward, 0.2f); // 0 = pure push, 1 = full forward

            // Apply velocity toward center
            float finalSpeed = speed;

            // Apply stamina boost
            if (isBoosting && currentStamina > 0f)
            {
                finalSpeed *= boostMultiplier;
            }

            // Apply power-up boost
            finalSpeed *= speedPowerUpMultiplier;

            rb.linearVelocity = finalDir.normalized * finalSpeed * 1.2f;
        }
    }

    //if isBoosting is true, apply the speed boost
    //if not, regen stamina and stop when filled to max
    private void HandleBoost()
    {
        if (isBoosting && currentStamina > 0f)
        {
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;

            if (staminaFillImage != null)
                staminaFillImage.color = Color.white;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isBoosting = false;
            }
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.fixedDeltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }

            if (staminaFillImage != null)
                staminaFillImage.color = originalColor;
        }

        if (staminaFillImage != null)
            staminaFillImage.fillAmount = currentStamina / maxStamina;
    }
    private void HandleRotation()
    {
        if (controlMode == ControlMode.Buttons)
        {
            if (turnDir != 0)
            {
                float rotationAmount = -turnDir * turnSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation + rotationAmount);
            }
        }
        else if (controlMode == ControlMode.Joystick && joystick != null)
        {
            float horizontalInput = joystick.Horizontal;

            if (Mathf.Abs(horizontalInput) > 0.1f)
            {
                float rotationAmount = -horizontalInput * turnSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation + rotationAmount);
            }
        }
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    // === ADD THIS ENTIRE METHOD to your PlayerMove script ===
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        speedPowerUpMultiplier = multiplier;
        speedBoostEndTime = Time.time + duration;
        isSpeedBoostActive = true;

        // Visual indicator: trail
        if (trail != null)
            trail.widthCurve = boostedWidthCurve;
    }

    //Functions for buttons to turn payer.
    public void TurnLeftDown() => turnDir = -1;
    public void TurnRightDown() => turnDir = 1;
    public void TurnUp() => turnDir = 0;

    public enum ControlMode
    {
        Buttons,
        Joystick
    }

    public void SetControlMode(ControlMode mode)
    {
        controlMode = mode;

        //Save Setting
        PlayerPrefs.SetInt("ControlMode", (int)mode);
        PlayerPrefs.Save();
    }

    public void BoostDown()
    {
        isBoosting = true;
    }
    public void BoostUp()
    {
        isBoosting = false;
    }
}