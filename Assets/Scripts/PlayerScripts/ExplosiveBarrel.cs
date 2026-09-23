 using UnityEngine;
using System.Collections;

public class ExplosiveBarrel : MonoBehaviour
{
    public float fuseTime = 3f;
    public float explosionRadius = 1f;
    public int damage = 30;
    private const string BarrelDamageKey = "BarrelDamageKey";

    public LayerMask enemyLayer;
    public LayerMask playerLayer;

    [Header("Explosion FX")]
    [SerializeField]
    private ParticleSystem explosion;
    [SerializeField]
    private ParticleSystem debris;

    private bool hasExploded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damage = PlayerPrefs.GetInt(BarrelDamageKey, damage);

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
        if(collision.CompareTag("Boss"))
        {
            BaseBoss boss = collision.GetComponent<BaseBoss>();
            boss.TakeDamage(damage);
            Explode();
        }
        if (collision.CompareTag("Player"))
        {
            onHitPlayer();
        }
    }

    private void PlayExplosionFX()
    {
        AudioManager.Instance.PlayExplosion();
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
            BossController boss = hit.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage); // adjust damage
                Destroy(gameObject);
            }
        }

        GameManager.Instance.ShakeCamera(0.25f, 0.2f);

        Destroy(gameObject);
    }

    void onHitPlayer()
    {
        PlayExplosionFX();

        if (hasExploded) return;
        hasExploded = true;

        //Damage all enemies in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerhp = hit.GetComponent<PlayerHealth>();
            if (playerhp != null)
            {
                playerhp.TakeDamage(damage);
                Debug.Log("Barrel Damage: " + damage);
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

    public static void IncreasePermanentBarrelDamage(int amount)
    {
        int currentDamage = PlayerPrefs.GetInt(BarrelDamageKey, 30);
        currentDamage += amount;

        PlayerPrefs.SetInt(BarrelDamageKey, currentDamage);
        PlayerPrefs.Save();

        Debug.Log("Permanent Barrel Damage Increased to: " + currentDamage);
    }
}
