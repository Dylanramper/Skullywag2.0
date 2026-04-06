using UnityEngine;

public class ShieldPowerUp : PowerUp
{
    public float duration = 5f;
    public int defenseBoost = 50;

    protected override void Apply(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ActivateShield(duration, defenseBoost);
        }
    }
}