using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ========== PUBLIC VARIABLES (Appear in Inspector) ==========
    [Header("Movement Settings")]
    public float defaultMoveSpeed = 5f;

    [Header("Speed Power-up Settings")]
    public float speedBoostMultiplier = 1.5f;
    public float speedBoostDuration = 3f;

    // ========== PRIVATE VARIABLES (Hidden in Inspector) ==========
    private float currentMoveSpeed;
    private float speedBoostEndTime = 0f;
    private bool isSpeedBoosted = false;

    // ========== UNITY METHODS ==========
    void Start()
    {
        // Initialize
        currentMoveSpeed = defaultMoveSpeed;
    }

    void Update()
    {
        // Check if speed boost expired
        if (isSpeedBoosted && Time.time > speedBoostEndTime)
        {
            EndSpeedBoost();
        }

        // Your existing movement code here
        HandleMovement();
    }

    // ========== CUSTOM METHODS ==========
    void HandleMovement()
    {
        // Example movement - REPLACE WITH YOUR ACTUAL MOVEMENT CODE
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        transform.Translate(movement * currentMoveSpeed * Time.deltaTime);
    }

    // Called by SpeedPowerUp script when collected
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        // Apply boost
        currentMoveSpeed = defaultMoveSpeed * multiplier;
        speedBoostEndTime = Time.time + duration;
        isSpeedBoosted = true;

        Debug.Log($"Speed Boost! {currentMoveSpeed} speed for {duration} seconds");

        // Optional: Visual/sound effects
    }

    void EndSpeedBoost()
    {
        currentMoveSpeed = defaultMoveSpeed;
        isSpeedBoosted = false;
        Debug.Log("Speed boost ended");

        // Optional: Visual effect removal
    }
}