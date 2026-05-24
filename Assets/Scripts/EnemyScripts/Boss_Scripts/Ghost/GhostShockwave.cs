using UnityEngine;

public class GhostShockwave : MonoBehaviour
{
    [Header("Expansion")]
    [SerializeField] private float expandSpeed = 8f;
    [SerializeField] private float maxScale = 12f;

    [Header("Damage")]
    [SerializeField] private int damage = 20;

    private bool hasHitPlayer;

    void Update()
    {
        ExpandWave();
    }

    void ExpandWave()
    {
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

        if(transform.localScale.x >= maxScale)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHitPlayer) return;
        Debug.Log("Hit Player");

        if (collision.CompareTag("Player"))
        {
            
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            hasHitPlayer = true;
        }
    }
}
