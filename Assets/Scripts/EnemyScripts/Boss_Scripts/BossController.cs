using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;

    public float mortarCooldown = 3f;
    public float cannonCooldown = 2f;
    public float barrelCooldown = 4f;

    private float mortarTimer;
    private float cannonTimer;
    private float barrelTimer;

    private float health;
    private float maxHealth = 100;

    public GameObject mortarIndicatorPrefab;
    public float mortarDelay = 1f;
    public GameObject explosionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePhase();
        HandleCooldowns();
        DecideAttack();
    }

    void UpdatePhase()
    {
        float healthPercent = health / maxHealth;

        if (healthPercent <= 0.3f)
        {
            // Phase 3
        }
        else if (healthPercent <= 0.7f)
        {
            // Phase 2
        }
        else
        {
            // Phase 1
        }
    }

    void DecideAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > 6f && mortarTimer <= 0f)
        {
            MortarAttack();
            mortarTimer = mortarCooldown;
        }
        else if (IsPlayerBehind() && barrelTimer <= 0f)
        {
            DropBarrel();
            barrelTimer = barrelCooldown;
        }
        else if (cannonTimer <= 0f)
        {
            SideCannons();
            cannonTimer = cannonCooldown;
        }
    }
        void MortarAttack()
        {
            Vector2 targetPos = player.position;

            GameObject indicator = Instantiate(mortarIndicatorPrefab, targetPos, Quaternion.identity);

            Destroy(indicator, mortarDelay);

            StartCoroutine(MortarExplosion(targetPos));
        }

    IEnumerator MortarExplosion(Vector2 position)
    {
        yield return new WaitForSeconds(mortarDelay);

        AudioManager.Instance.PlayExplosion();
        AudioManager.Instance.PlayHit();

        Instantiate(explosionPrefab, position, Quaternion.identity);
    }

    void DropBarrel()
    {
        Debug.Log("Barrel Dropped");
    }

    void SideCannons()
    {
        Debug.Log("Cannons Fired");
    }

    void HandleCooldowns()
    {
        mortarTimer -= Time.deltaTime;
        cannonTimer -= Time.deltaTime;
        barrelTimer -= Time.deltaTime;
    }

    bool IsPlayerBehind()
    {
        Vector2 toPlayer = (player.position - transform.position).normalized;

        float dot = Vector2.Dot(transform.up, toPlayer);

        // If dot is negative, player is behind
        return dot < -0.3f;
    }
}
