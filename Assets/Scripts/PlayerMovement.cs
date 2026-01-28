using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Power-up")]
    private float currentMoveSpeed;
    private float speedBoostEndTime = 0f; // Time when the boost wears off

    private Rigidbody2D rb;
    public float speed;
    public float turnSpeed;
    private int turnDir = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 1.5f;
        currentMoveSpeed = speed;
    }

    private void FixedUpdate()
    {
        Debug.Log(speed);
        // Check if a speed boost has expired
        if (Time.time > speedBoostEndTime && currentMoveSpeed != speed)
        {
            // Boost just ended
            Debug.Log("Speed boost ended!");
            currentMoveSpeed = speed;
        }

        //Move Forward 
        rb.linearVelocity = (Vector2)transform.up * speed;

        //If the player is pressing left or right buttons on screen; calculate rotation and speed.
        //Rotate the player
        if(turnDir != 0) 
        {
            float rotationAmount = -turnDir * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation + rotationAmount);

            float targetRotation = rb.rotation + rotationAmount;
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, 0.9f));
        }
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
}
