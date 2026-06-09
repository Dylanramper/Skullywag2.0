using UnityEngine;

public class GhostBoat : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 120f;

    [SerializeField] private int damage = 5;
    [SerializeField] private float lifeTime = 8f;

    private Transform player;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if(p != null)
           player = p.transform;
        
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if(player == null) return;

        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;

        float angle = Vector2.SignedAngle(transform.up, toPlayer);

        rb.MoveRotation(rb.rotation + angle * turnSpeed * Time.fixedDeltaTime / 90f);

        rb.linearVelocity = transform.up * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.CompareTag("Player")) 
           return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if(playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            //------------------------------------------------------------------------SLOW EFFECT------------------------
        }
        Destroy(gameObject);
    }
}
