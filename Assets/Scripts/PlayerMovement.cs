using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float turnSpeed = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 1.5f;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = (Vector2)transform.up * speed;
    }
}
