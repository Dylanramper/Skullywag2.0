using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem FX1;
    public float damage = 20f;
    public float lifetime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        FX1 = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            other.GetComponent<PlayerHealth>()?.TakeDamage((int)damage);
            FX1.Play();
        }
    }
}