using UnityEngine;

public class TentacleDamage : MonoBehaviour
{
    private CursedBoss cursedBoss;

    private void Awake()
    {
        cursedBoss = GetComponentInParent<CursedBoss>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            cursedBoss.TentacleHitPlayer();
        }
    }
}
