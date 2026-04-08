using UnityEngine;

public class SpeedPowerUp : PowerUp
{
    [Header("Speed Settings")]
    public float speedMultiplier = 1.5f;
    public float duration = 3f;

    protected override void Apply(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.ApplySpeedBoost(speedMultiplier, duration);
        }
    }
}