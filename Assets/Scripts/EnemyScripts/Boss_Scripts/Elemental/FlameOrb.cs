using System.Collections;
using UnityEngine;

public class FlameOrb : MonoBehaviour
{
    private Transform boss;
    private float angle;
    private float radius = 1.5f;
    private float orbitSpeed = 180f;

    private bool isLaunched = false;
    private Vector2 velocity;

    [Header("Launch")]
    public float launchSpeed = 6f;
    public float turnSpeed = 4f;

    public void Initialize(Transform bossTransform, float startAngle)
    {
        boss = bossTransform;
        angle = startAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLaunched)
        {
            Orbit();
        }
        else
        {
            Fly();
        }
    }

    void Orbit()
    {
        if (boss == null) return;

        angle -= orbitSpeed * Time.deltaTime;

        Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

        transform.position = (Vector2)boss.position + offset;
    }

    public void Launch(Transform player)
    {
        isLaunched = true;

        velocity = transform.up * launchSpeed;

        StartCoroutine(HomeTowards(player));
        Destroy(gameObject, 4f);
    }

    void Fly()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    IEnumerator HomeTowards(Transform player)
    {
        float timer = 0.5f;

        while(timer > 0 && player != null)
        {
            Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

            velocity = Vector2.Lerp(velocity, toPlayer * launchSpeed, turnSpeed * Time.deltaTime);

            timer -= Time.deltaTime;

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(10);
            Destroy(gameObject);
        }
    }
}
