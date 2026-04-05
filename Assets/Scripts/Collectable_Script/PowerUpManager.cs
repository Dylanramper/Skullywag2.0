using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("Limits")]
    public int maxActivePowerUps = 5;

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
        if (activePowerUps.Count >= maxActivePowerUps)
            return;

        GameObject powerUp = Instantiate(prefab, position, Quaternion.identity);
        activePowerUps.Add(powerUp);
    }

    public void SpawnWithPush(GameObject prefab, Vector3 position, float minForce = 2f, float maxForce = 4f)
    {
        if(activePowerUps.Count >= maxActivePowerUps)
            return;

        GameObject powerUp = Instantiate(prefab, position, Quaternion.identity);
        activePowerUps.Add(powerUp);

        Rigidbody2D rb = powerUp.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(dir * force, ForceMode2D.Impulse);
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