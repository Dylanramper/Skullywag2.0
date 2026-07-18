using System.Collections;
using UnityEngine;

public class GhostBoat : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float turnSpeed = 120f;

    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 8f;

    private Transform player;
    private Rigidbody2D rb;
    private bool canMove = false;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem spawnMist;
    [SerializeField] private ParticleSystem deathExplosion;

    private Vector3 originalScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if(p != null)
           player = p.transform;
        
        StartCoroutine(LifeTime());

        StartCoroutine(SpawnRoutine());
    }

    private void FixedUpdate()
    {
        if(!canMove) return;

        if (player == null) return;

        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;

        float angle = Vector2.SignedAngle(transform.up, toPlayer);

        rb.MoveRotation(rb.rotation + angle * turnSpeed * Time.fixedDeltaTime / 90f);

        rb.linearVelocity = transform.up * moveSpeed;
    }

    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        if (deathExplosion != null)
        {
            deathExplosion.transform.parent = null;
            deathExplosion.Play();
        }
        Destroy(gameObject);
    }

    private IEnumerator SpawnRoutine()
    {
        canMove = false;

        if(spriteRenderer == null)
            spriteRenderer.GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;

        Vector3 startScale = originalScale * 1.3f;

        transform.localScale = startScale;

        //Start Invisible
        Color color = spriteRenderer.color;
        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        //Play Mist Particle Effect
        if(spawnMist != null)
            spawnMist.Play();

        float timer = 0f;
        float duration = 1.25f;

        //Fade in and scale down to original size
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            transform.localScale = Vector3.Lerp(startScale, originalScale, t);

            spriteRenderer.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0f, 1f, t));

            yield return null;
        }

        //Hold before attacking
        yield return new WaitForSeconds(2.2f);

        canMove = true;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.CompareTag("Player")) 
           return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        if(playerMovement != null)
        {
            playerMovement.ApplySlow(0.6f, 3f);
        }

        if(deathExplosion != null)
        {
            deathExplosion.transform.parent = null;
            deathExplosion.Play();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (deathExplosion != null)
        {
            deathExplosion.transform.parent = null;
            deathExplosion.Play();
        }
        Destroy(gameObject);
    }
}
