using UnityEngine;

public class PowerUpCounter : MonoBehaviour
{
    // This tracks ALL power-ups on screen
    public static int TotalPowerUpsOnScreen = 0;
    public static int MaxPowerUpsAllowed = 6;

    void Start()
    {
        // Add this power-up to the count
        TotalPowerUpsOnScreen++;

        // Check if we have too many
        if (TotalPowerUpsOnScreen > MaxPowerUpsAllowed)
        {
            Destroy(gameObject); // Destroy this one
        }
    }

    void OnDestroy()
    {
        // Remove from count when destroyed
        TotalPowerUpsOnScreen--;
        if (TotalPowerUpsOnScreen < 0) TotalPowerUpsOnScreen = 0;
    }
}