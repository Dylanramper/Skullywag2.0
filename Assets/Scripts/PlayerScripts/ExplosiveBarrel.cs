using UnityEngine;
using System.Collections;

public class ExplosiveBarrel : MonoBehaviour
{
    public float fuseTime = 3f;
    public float explosionRadius = 1f;
    public int damage = 30;

    public LayerMask enemyLayer;
    public GameObject explosionEffect;

    [Header("Explosion FX")]
    [SerializeField]
    private ParticleSystem explosion;
    [SerializeField]
    private ParticleSystem debris;

    private bool hasExploded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FuseTimer());
    }

    IEnumerator FuseTimer()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    private void PlayExplosionFX()
    {
        if (explosion != null)
        {
            explosion.transform.parent = null;
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration + explosion.main.startLifetime.constantMax);
        }
        if (debris != null)
        {
            debris.transform.parent = null;
            debris.Play();
            Destroy(debris.gameObject, debris.main.duration + debris.main.startLifetime.constantMax);
        }
    }

    void Explode()
    {
        PlayExplosionFX();

        if (hasExploded) return;
        hasExploded = true;

        //Damage all enemies in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach(Collider2D hit in hits)
        {
            EnemyShip enemy = hit.GetComponent<EnemyShip>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        GameManager.Instance.ShakeCamera(0.25f, 0.2f);

        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
