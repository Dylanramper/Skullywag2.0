using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float turnSpeed;
    private int turnDir = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 1.5f;
    }

    private void FixedUpdate()
    {
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

    //Functions for buttons to turn payer.
    public void TurnLeftDown() => turnDir = -1;
    public void TurnRightDown() => turnDir = 1;
    public void TurnUp() => turnDir = 0;
}
