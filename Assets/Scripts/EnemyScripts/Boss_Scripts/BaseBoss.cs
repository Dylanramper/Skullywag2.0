using UnityEngine;

public abstract class BaseBoss : MonoBehaviour
{
    protected float health;
    protected float maxHealth = 100f;

    protected int currentPhase = 1;
    protected int previousPhase = 1;

    protected Transform player;

    protected virtual void Start()
    {
        health = maxHealth;
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
        health -= damage;

        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        //Destroy(gameObject);
    }

    public float GetHealthPercent()
    {
        return health / maxHealth;
    }
}