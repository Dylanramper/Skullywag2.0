using UnityEngine;

public class FlameProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float lifetime = 0.4f;

    [Header("Spread")]
    public float spreadAngle = 8f;

    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        //Slight random spread
        float angle = Random.Range(-spreadAngle, spreadAngle);
        direction = Quaternion.Euler(0, 0, angle) * dir;

        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //deal damage to player-------------------------------------------------------------------------------------
            Debug.Log("Flame Damage");
        }
    }
}
