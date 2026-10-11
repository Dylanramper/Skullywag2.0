
using UnityEngine;

public abstract class BaseBoss : MonoBehaviour
{
    public float health;
    protected float maxHealth = 100f;

    [Header("Boss UI")]
    [SerializeField] private BossHPBar bossHealthbar;

    protected int currentPhase = 1;
    protected int previousPhase = 1;

    protected Transform player;

    protected virtual void Start()
    {
        health = maxHealth;

        // Find the shared boss indicator.
        BossIndicator indicator = FindFirstObjectByType<BossIndicator>();

        if (indicator != null)
            indicator.SetBoss(transform);

        UpdateHealthBar();
    }

    public void SetBossHealthbar(BossHPBar hpBar)
    {
        bossHealthbar = hpBar;
        UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        if (bossHealthbar != null)
            bossHealthbar.UpdateHealth(health, maxHealth);
    }

    protected void ClearBossIndicator()
    {
        BossIndicator indicator = FindFirstObjectByType<BossIndicator>();

        if (indicator != null && indicator.boss == transform)
            indicator.ClearBoss();
    }

    protected virtual void Update()
    {
        UpdatePhase();
    }

    protected virtual void UpdatePhase()
    {
        float healthPercent = health / maxHealth;

        int newPhase;

        if (healthPercent <= 0.3f)
            newPhase = 3;
        else if (healthPercent <= 0.7f)
            newPhase = 2;
        else
            newPhase = 1;

        if (newPhase != currentPhase)
            OnPhaseChanged(newPhase);

        previousPhase = currentPhase;
        currentPhase = newPhase;
    }

    protected virtual void OnPhaseChanged(int newPhase) { }

    public virtual void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public virtual void TakeDamage(int damage)
    {
        health = Mathf.Max(0f, health - damage);

        UpdateHealthBar();
        AudioManager.Instance.PlayHit();
        GameManager.Instance.ShakeCamera(0.1f, 0.08f);


        if (health <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        ClearBossIndicator();
        Destroy(gameObject);
    }

    public float GetHealthPercent()
    {
        return maxHealth > 0f ? health / maxHealth : 0f;
    }
}
