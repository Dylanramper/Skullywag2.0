using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public PowerUpData data;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Apply(other.gameObject);

        if (PowerUpUIManager.Instance != null && data.icon != null)
        {
            PowerUpUIManager.Instance.ShowPowerUpIcon(data.icon);
        }

        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.Remove(gameObject);
        }

        Destroy(gameObject);
    }

    protected abstract void Apply(GameObject player);
}