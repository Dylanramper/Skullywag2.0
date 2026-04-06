using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName;
    public GameObject prefab;
    public Sprite icon;

    [Range(0f, 100f)]
    public float spawnWeight = 10f;
}