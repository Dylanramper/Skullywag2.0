using UnityEngine;
using System.Collections;

public class ElementalBoss : BaseBoss
{
    [Header("Scatter Mortar Shot")]
    [SerializeField] private GameObject scatterProjectilePrefab;
    [SerializeField] private int scatterShotCount = 10;
    [SerializeField] private float scatterRadius = 6f;
    [SerializeField] private float scatterDelayBetweenShots = 0.2f;

    private enum BossState
    {
        Idle,
        ScatterShot,
        NapalmAttack,
        FlameBurst
    }

    private BossState currentState;

    private float stateTimer;

    protected override void Start()
    {
        base.Start();

        currentState = BossState.Idle;
    }

    protected override void Update()
    {
        base.Update();

        HandleState();
    }
    void HandleState()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Idle:
                ChooseNextAttack();
                break;

            case BossState.ScatterShot:
                if (stateTimer <= 0)
                    ChangeState(BossState.NapalmAttack);
                break;

            case BossState.NapalmAttack:
                if (stateTimer <= 0)
                    ChangeState(BossState.FlameBurst);
                break;

            case BossState.FlameBurst:
                if (stateTimer <= 0)
                    ChangeState(BossState.Idle);
                break;
        }
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BossState.Idle:
                stateTimer = 1.5f;
                break;

            case BossState.ScatterShot:
                StartScatterShot();
                break;

            case BossState.NapalmAttack:
                StartNapalmAttack();
                break;

            case BossState.FlameBurst:
                StartFlameBurst();
                break;
        }
    }

    void StartScatterShot()
    {
        stateTimer = 3f;

        StartCoroutine(ScatterShotRoutine());
    }
    IEnumerator ScatterShotRoutine()
    {
        if (player == null) yield break;

        for (int i = 0; i < scatterShotCount; i++)
        {
            Vector2 target = GetScatterTarget();

            SpawnScatterProjectile(target);

            yield return new WaitForSeconds(scatterDelayBetweenShots);
        }
    }

    Vector2 GetScatterTarget()
    {
        Vector2 playerPos = player.position;

        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(1.5f, scatterRadius);

        Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

        return playerPos + offset;
    }
    void SpawnScatterProjectile(Vector2 target)
    {
        if (scatterProjectilePrefab == null) return;

        Vector2 spawnPos = transform.position;

        GameObject proj = Instantiate(scatterProjectilePrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (target - spawnPos).normalized;

            float force = 8f; // tweak later
            rb.linearVelocity = dir * force;
        }
    }

    void StartNapalmAttack()
    {
        stateTimer = 4f;

        // TODO: Spawn napalm zones
    }

    void StartFlameBurst()
    {
        stateTimer = 2f;

        // TODO: Flame cone attack
    }

    void ChooseNextAttack()
    {
        int choice = Random.Range(0, 3);

        if (choice == 0)
            ChangeState(BossState.ScatterShot);
        else if (choice == 1)
            ChangeState(BossState.NapalmAttack);
        else
            ChangeState(BossState.FlameBurst);
    }

    protected override void OnPhaseChanged(int newPhase)
    {
        Debug.Log("Boss entered phase: " + newPhase);

        switch (newPhase)
        {
            case 2:
                // Increase aggression
                break;

            case 3:
                // Go crazy mode
                break;
        }
    }
}