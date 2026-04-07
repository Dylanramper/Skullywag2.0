using UnityEngine;

public class DoubleDamagePowerUp : PowerUp
{
    [Header("Settings")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float damageMultiplier = 2f;

    protected override void Apply(GameObject player)
    {
        PlayerCannons cannons = player.GetComponent<PlayerCannons>();

        if (cannons != null)
        {
            // Only boost damage, not fire rate
            cannons.ApplyWeaponBoost(1f, damageMultiplier, duration);
        }
        else
        {
            Debug.LogWarning("PlayerCannons not found on player!");
        }
    }
}