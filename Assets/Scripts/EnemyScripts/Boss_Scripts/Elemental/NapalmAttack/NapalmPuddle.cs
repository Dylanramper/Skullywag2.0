using UnityEngine;

public class NapalmPuddle : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private float damageRadius = 1.5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Deal damage in PlayerHealth.cs later (DPS) --------------------------------------------------------------
            Debug.Log("Napalm dmg");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
