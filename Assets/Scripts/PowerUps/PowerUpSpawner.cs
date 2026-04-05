using UnityEngine;
using UnityEngine.InputSystem;

public class TestSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Vector3 pos = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0f);

            PowerUpManager.Instance.Spawn(powerUpPrefab, pos);

            Debug.Log("Spawned Power up");

        }
    }
}