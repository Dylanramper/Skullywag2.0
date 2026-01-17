using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float turnSpeed;
    private int turnDir = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 1.5f;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = (Vector2)transform.up * speed;

        if(turnDir != 0) 
        {
            float rotationAmount = -turnDir * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation + rotationAmount);

            float targetRotation = rb.rotation + rotationAmount;
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetRotation, 0.9f));
        }
    }

    public void TurnLeftDown() => turnDir = -1;
    public void TurnRightDown() => turnDir = 1;
    public void TurnUp() => turnDir = 0;
}
