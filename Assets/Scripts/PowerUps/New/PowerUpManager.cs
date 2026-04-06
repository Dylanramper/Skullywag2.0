using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("Limits")]
    public int maxActivePowerUps = 5;

    [Header("Available PowerUps")]
    public List<PowerUpData> availablePowerUps; // Drag your ShieldPowerUpData etc here

    private List<GameObject> activePowerUps = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Spawn(GameObject prefab, Vector3 position)
    {
        if (activePowerUps.Count >= maxActivePowerUps) return;

        GameObject powerUp = Instantiate(prefab, position, Quaternion.identity);
        activePowerUps.Add(powerUp);
    }

    public void SpawnWithPush(GameObject prefab, Vector3 position, float minForce = 2f, float maxForce = 4f)
    {
        if (activePowerUps.Count >= maxActivePowerUps) return;

        GameObject powerUp = Instantiate(prefab, position, Quaternion.identity);
        activePowerUps.Add(powerUp);

        Rigidbody2D rb = powerUp.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }

    //Spawn a random power-up from the list
    public void SpawnRandom(Vector3 position, bool push = false)
    {
        if (availablePowerUps.Count == 0 || activePowerUps.Count >= maxActivePowerUps)
            return;

        int index = Random.Range(0, availablePowerUps.Count);
        PowerUpData data = availablePowerUps[index];

        if (data != null && data.prefab != null)
        {
            if (push)
                SpawnWithPush(data.prefab, position);
            else
                Spawn(data.prefab, position);
        }
    }

    public void Remove(GameObject powerUp)
    {
        if (activePowerUps.Contains(powerUp))
        {
            activePowerUps.Remove(powerUp);
        }
    }
}