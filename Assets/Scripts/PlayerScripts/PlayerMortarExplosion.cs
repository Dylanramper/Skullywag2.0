
using System.Collections.Generic;
using UnityEngine;

public class PlayerMortarExplosion : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float radius = 1.5f;

    private void Start()
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, radius);

        HashSet<EnemyShip> damagedEnemies = new HashSet<EnemyShip>();
        HashSet<BaseBoss> damagedBosses = new HashSet<BaseBoss>();

        foreach (Collider2D hit in hits)
        {
            EnemyShip enemy = hit.GetComponentInParent<EnemyShip>();

            if (enemy != null && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage));
                continue;
            }

            BaseBoss boss = hit.GetComponentInParent<BaseBoss>();

            if (boss != null && damagedBosses.Add(boss))
            {
                boss.TakeDamage(Mathf.RoundToInt(damage));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
