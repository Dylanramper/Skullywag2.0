using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class GhostBoss : BaseBoss
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 80f;
    [SerializeField] private float idealRange = 7f;
    [SerializeField] private float tooCloseRange = 4f;

    private Rigidbody2D rb;

    [Header("Teleport")]
    [SerializeField] private float teleportCooldown = 5f;

    private float stateTimer;
    private bool isTeleporting;

    private enum BossState
    {
        Chasing, Teleporting, Attacking
    }

    private BossState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentState = BossState.Chasing;
        stateTimer = teleportCooldown;

        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
            {
                player = p.transform;
            }
        }

        currentState = BossState.Chasing;
    }

    protected override void Update()
    {
        base.Update();

        if (player == null) return;

        HandleState();
    }

    void HandleState()
    {
        stateTimer -= Time.deltaTime;
        switch(currentState)
        {
            case BossState.Chasing:
                ShipMovement();
                if(stateTimer <= 0 && !isTeleporting)
                {
                    ChangeState(BossState.Teleporting);
                }
                break;

            case BossState.Teleporting:
                //ShipMovement();
                break;

            case BossState.Attacking:
                ShipMovement();
                break;
        }
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BossState.Chasing:
                stateTimer = teleportCooldown;
                break;

            case BossState.Teleporting:
                StartCoroutine(TeleportRoutine());
                break;
            case BossState.Attacking:
                break;
        }
    }

    IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        Debug.Log("Teleport Started");

        yield return new WaitForSeconds(2f);

        Debug.Log("Teleport Finished");

        isTeleporting = false;

        ChangeState(BossState.Chasing);
    }

    void ShipMovement()
    {
        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        float distance = Vector2.Distance(transform.position, player.position);

        transform.position += transform.up * moveSpeed * Time.deltaTime;

        float angleToPlayer = Vector2.SignedAngle(transform.up, toPlayer);

        float turnDirection = Mathf.Sign(angleToPlayer);

        float turnAmount = turnDirection * turnSpeed * Time.deltaTime;

        transform.Rotate(0, 0, turnAmount);

        if(distance < tooCloseRange)
        {
            transform.position -= transform.up * (moveSpeed * 0.4f * Time.deltaTime);
        }
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }
}
