using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float dot = 5f;
    [SerializeField] private float burnDuration = 2f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();

            if(player != null )
            {
                player.ApplyBurn(dot, burnDuration);
            }
        }
    }
}
