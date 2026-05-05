using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class ElementalBoss : BaseBoss
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float turnSpeed = 80f;

    [SerializeField] private float idealRange = 6f;
    [SerializeField] private float tooCloseRange = 3.5f;

    [SerializeField] private float broadsideAngle = 90f;
    [SerializeField] private float angleTolerance = 8f;

    [Header("Scatter Mortar Shot")]
    [SerializeField] private GameObject scatterProjectilePrefab;

    [SerializeField] private ReticleSpawner reticleSpawner;
    [SerializeField] private Transform scatterPoint;

    [Header("Napalm Attack")]
    [SerializeField] private NapalmEmitter napalmEmitter;


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

        if(player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if(p != null)
            {
                player = p.transform;
            }
        }

        currentState = BossState.Idle;
    }

    protected override void Update()
    {
        base.Update();

        HandleState();
        HandleMovement();

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TestScatterAttack();
        }

    }

    void HandleMovement()
    {
        if (player == null) return;

        switch (currentState)
        {
            case BossState.Idle:
                ShipMovement();
                break;
            case BossState.ScatterShot:
                ShipMovement();
                break;
            case BossState.NapalmAttack:
                ShipMovement();
                break;
            case BossState.FlameBurst:
                //PlaceHolder-----------Make new ability----------------------------
                ShipMovement();
                break;
        }
    }

    void ShipMovement()
    {
        if(player == null) return;

        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        //Constant forward movement
        transform.position += transform.up * moveSpeed * Time.deltaTime;
        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);
        float turnDirection = Mathf.Sign(angleToPlayer);

        if(currentState == BossState.NapalmAttack)
        {
            MaintainBroadside(angleToPlayer);
        }
        else
        {
            //Turn To player
            float turnAmount = turnDirection * turnSpeed * 0.6f * Time.deltaTime;
            transform.Rotate(0, 0, turnAmount);
        }

        if(distance < tooCloseRange)
        {
            //Slow down if too close to player
            transform.position -= transform.up * (moveSpeed * 0.5f * Time.deltaTime);
        }
    }

    void MaintainBroadside(float angleToPlayer)
    {
        float targetAngle = Mathf.Abs(angleToPlayer - broadsideAngle) < Mathf.Abs(angleToPlayer + broadsideAngle) ? broadsideAngle : -broadsideAngle;

        float angleDiff = Mathf.DeltaAngle(angleToPlayer, targetAngle);

        float turnDir = -Mathf.Sign(angleDiff);

        transform.Rotate(0, 0, turnDir * turnSpeed * 0.5f * Time.deltaTime);
    }

    void HandleState()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0)
                    ChooseNextAttack();
                break;

            case BossState.ScatterShot:
                if (stateTimer <= 0)
                    ChangeState(BossState.NapalmAttack);
                break;

            case BossState.NapalmAttack:
                if (stateTimer <= 0)
                {
                    moveSpeed /= 1.3f;
                    napalmEmitter.StopFiring();
                    ChangeState(BossState.FlameBurst);
                }
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

        StartCoroutine(ScatterAttackRoutine());
    }
    IEnumerator ScatterAttackRoutine()
    {
        if(player == null)
        {
            Debug.Log("Player is Null");
            yield break;
        }
        Debug.Log("ScatterShot STARTED");

        //Spawn Reticles
        reticleSpawner.SpawnReticles();

        //Delay before attacking (Warning indicator)
        yield return new WaitForSeconds(0.8f);

        Vector2 target = Vector2.zero;

        foreach(var pos in reticleSpawner.reticlePositions)
        {
            target += pos;
        }

        target /= reticleSpawner.reticlePositions.Count;

        //Fire scatter container projectile
        GameObject proj = Instantiate(scatterProjectilePrefab, scatterPoint.position, Quaternion.identity);

        proj.GetComponent<ScatterProjectile>().Initialize(scatterPoint.position, target, reticleSpawner.reticlePositions);
    }
    void StartNapalmAttack()
    {
        stateTimer = 4f;

        moveSpeed *= 1.3f;
        napalmEmitter.StartFiring();
    }

    void StartFlameBurst()
    {
        stateTimer = 2f;

        // TODO: Flame cone attack
    }

    void ChooseNextAttack()
    {
        ChangeState(BossState.ScatterShot);
        /*
        int choice = Random.Range(0, 3);

        if (choice == 0)
            ChangeState(BossState.ScatterShot);
        else if (choice == 1)
            ChangeState(BossState.NapalmAttack);
        else
            ChangeState(BossState.FlameBurst); */
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

    void TestScatterAttack()
    {
        // Step 1: Spawn reticles
        reticleSpawner.SpawnReticles();

        // Step 2: Get center target (player area)
        Vector2 target = Vector2.zero;

        foreach (var pos in reticleSpawner.reticlePositions)
        {
            target += pos;
        }

        target /= reticleSpawner.reticlePositions.Count;

        // Step 3: Spawn projectile
        GameObject proj = Instantiate(scatterProjectilePrefab, scatterPoint.position, Quaternion.identity);

        proj.GetComponent<ScatterProjectile>().Initialize(scatterPoint.position, target, reticleSpawner.reticlePositions);
    }
}