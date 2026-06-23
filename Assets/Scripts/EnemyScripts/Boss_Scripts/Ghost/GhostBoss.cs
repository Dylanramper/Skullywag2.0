using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class GhostBoss : BaseBoss
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PolygonCollider2D col;
    private Vector3 originalScale;

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
    private TrailRenderer trailRenderer;

    [Header("Shockwave")]
    [SerializeField] private GameObject shockwavePrefab;

    [Header("Ghost Boats")]
    [SerializeField] private GameObject ghostBoatPrefab;
    [SerializeField] private int boatsPerSummon = 4;
    [SerializeField] private float summonCooldown = 8f;

    private enum BossState
    {
        Chasing, Teleporting, Attacking, Summoning
    }

    private BossState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentState = BossState.Chasing;
        stateTimer = teleportCooldown;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        originalScale = transform.localScale;

        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

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

            case BossState.Summoning:
                ShipMovement();
                if (stateTimer <= 0)
                {
                    ChangeState(BossState.Summoning);
                }
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

    IEnumerator SummonRoutine()
    {
        Debug.Log("Summoning Minions!");

        yield new WaitForSeconds(1f);

        SpawnGhostBoats();

        ChangeState(BossState.Chasing);
    }

    void SpawnGhostBoats()
    {
        for(int i = 0; i < boatsPerSummon; i++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;

            Vector2 spawnPos = (Vector2)player.position + direction * Random.Range(6f, 8f);

            Instantiate(ghostBoatPrefab, spawnPos, Quaternion.identity);
        }
    }//--------------------------------------------------------------------LEFT OFF HERE---------------------------

    IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        Debug.Log("Teleport Started");

        //Scale up to emulate hovering up
        float timer = 0f;
        float duration = 0.6f;

        Vector3 targetScale = originalScale * 1.5f;

        while(timer < duration)
        {
            timer += Time.deltaTime;

            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);

            yield return null;
        }
        //Disappear
        col.enabled = false;
        trailRenderer.enabled = false;
        Color color = spriteRenderer.color;

        timer = 0f;
        duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / duration);

            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        //Reposition
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        float randomDistance = Random.Range(4f, 5f);

        Vector2 teleportPosition = (Vector2)player.position + randomDirection * randomDistance;

        transform.position = teleportPosition;

        //Reappear
        timer = 0f;
        duration = 0.4f; 

        while(timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, timer / duration);

            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        //Scale back down to emulate a slamming motion on the water
        timer = 0f;
        duration = 0.3f;

        while(timer < duration)
        {
            timer += Time.deltaTime;

            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / duration);

            yield return null;
        }
        Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        GameManager.Instance.ShakeCamera(0.3f, 0.25f);

        col.enabled = true;
        trailRenderer.enabled = true;

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
