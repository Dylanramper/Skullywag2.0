using UnityEngine;
using System.Collections.Generic;

public class TentacleDetection : MonoBehaviour
{
    public enum TentacleSide
    {
        RT, RM, RB, LT, LM, LB
    }

    [SerializeField] private CursedBoss cursedBoss;
    [SerializeField] private TentacleSide tentacleSide;

    private HashSet<Collider2D> playerColliders = new HashSet<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        bool wasEmpty = playerColliders.Count == 0;
        playerColliders.Add(collision);

        if (wasEmpty && playerColliders.Count > 0)
            cursedBoss.SetTentacleNearby(tentacleSide, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerColliders.Remove(collision);

        if(playerColliders.Count == 0)
            cursedBoss.SetTentacleNearby(tentacleSide, false);
    }
}
