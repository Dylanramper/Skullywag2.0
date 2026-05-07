using UnityEngine;
using System.Collections;

public class NapalmEmitter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private GameObject napalmPuddlePrefab;
    [SerializeField] private GameObject napalmProjectilePrefab;
    [SerializeField] private NapalmFlameFX leftFlame;
    [SerializeField] private NapalmFlameFX rightFlame;

    [Header("Settings")]
    [SerializeField] private float spawnInterval = 0.15f;
    [SerializeField] private float range = 6f;
    [SerializeField] private float spreadAngle = 8f;
    [SerializeField] private float damagePerSecond = 10f;

    private Transform player;
    private Coroutine fireRoutine;
    public bool IsFiring => fireRoutine != null;

    private void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        player = p.transform;
    }

    public void StartFiring()
    {
        if(fireRoutine == null)
        {
            fireRoutine = StartCoroutine(FireRoutine());
        }
    }

    public void StopFiring()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }

        leftFlame.StopFiring();
        rightFlame.StopFiring();
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.position);

                if (dist <= range)
                {
                    leftFlame.StartFiring();
                    rightFlame.StartFiring();

                    DealDamage();
                }
                else
                {
                    leftFlame.StopFiring();
                    rightFlame.StopFiring();
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnNapalm(Transform point, Vector2 direction)
    {
        GameObject flame = Instantiate(napalmProjectilePrefab, point.position, Quaternion.identity);

        flame.GetComponent<FlameProjectile>().Initialize(direction);
    }

    void DealDamage()
    {
        if (player == null) return;

        //Deal Damage from PlayerHealth.cs-------------------------------------------------------------------------------------------------------
    }
}
