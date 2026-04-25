using System.Collections.Generic;
using UnityEngine;

public class ScatterShotSpawner : MonoBehaviour
{
    [SerializeField] private GameObject scatterShotBallPrefab;

    public void Initialize(List<Vector2> targetPositions)
    {
        foreach (var target in targetPositions)
        {
            GameObject shot = Instantiate(scatterShotBallPrefab, transform.position, Quaternion.identity);

            shot.GetComponent<ScatterShotBall>()
                .Launch(transform.position, target);
        }

        // Destroy self after spawning everything
        Destroy(gameObject);
    }
}