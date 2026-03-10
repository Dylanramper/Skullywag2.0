using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

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

    [Header("Speed Power-up")]
    private float currentMoveSpeed;
    private float speedBoostEndTime = 2f; // Time when the boost wears off

    private Rigidbody2D rb;
    public float speed;
    public float turnSpeed;
    private int turnDir = 0;

    [SerializeField] private ControlMode controlMode = ControlMode.Buttons;
    [SerializeField] private Joystick joystick;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = 1.5f;
        speed = currentMoveSpeed;
        currentStamina = maxStamina;
        originalColor = staminaFillImage.color;

        //Load Save data(Previously used control scheme)
        if (PlayerPrefs.HasKey("ControlMode"))
        {
            controlMode = (ControlMode)PlayerPrefs.GetInt("ControlMode");
        }
    }

    private void FixedUpdate()
    {
        // Check if a speed boost has expired
        if (Time.time > speedBoostEndTime && currentMoveSpeed != speed)
        {
            // Boost just ended
            Debug.Log("Speed boost ended!");
            currentMoveSpeed = speed;
        }

        //Method for handling the boost button and stamina regain/drain
        HandleBoost();

        //Move Forward 
        rb.linearVelocity = (Vector2)transform.up * currentMoveSpeed;

        //If the player is pressing left or right buttons on screen or is using the joystick; calculate rotation and speed.
        //Rotate the player

        if(controlMode == ControlMode.Buttons)
        {
            if(turnDir != 0)
            {
                float rotationAmount = -turnDir * turnSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation + rotationAmount);
            }
        }
        else if (controlMode == ControlMode.Joystick && joystick != null)
        {
            float horizontalInput = joystick.Horizontal;

            if(Mathf.Abs(horizontalInput) > 0.1f)
            {
                float rotationAmount = -horizontalInput * turnSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation + rotationAmount);
            }
        } 
    }

    //if isBoosting is true, apply the speed boost
    //if not, regen stamina and stop when filled to max
    private void HandleBoost()
    {
        if(isBoosting && currentStamina > 0f)
        {
            currentMoveSpeed = speed * boostMultiplier;
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;

            if(staminaFillImage != null) 
                staminaFillImage.color = Color.white;

            if(currentStamina <= 0f)
            {
                currentStamina = 0f;
                isBoosting = false;
            }
        }
        else
        {
            currentMoveSpeed = speed;
            if(currentStamina < maxStamina)
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

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    // === ADD THIS ENTIRE METHOD to your PlayerMove script ===
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        // Calculate new speed
        currentMoveSpeed = speed * multiplier;

        // Set the time when the boost should end
        speedBoostEndTime = Time.time + duration;

        // Optional: Visual/audio feedback on the player
        Debug.Log($"Speed boosted! Current speed: {currentMoveSpeed} (Boost ends in {duration}s)");

        // Optional: You could trigger a particle effect on the player here
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
