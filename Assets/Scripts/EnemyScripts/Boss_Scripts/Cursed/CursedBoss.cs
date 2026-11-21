using UnityEngine;

public class CursedBoss : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool[] tentacleNearby = new bool[6];
    private bool[] tentacleAttacking = new bool[6];

    [Header("Tentacle Colliders")]
    [SerializeField] private Transform RT;
    [SerializeField] private Transform RM;
    [SerializeField] private Transform RB;
    [SerializeField] private Transform LT;
    [SerializeField] private Transform LM;
    [SerializeField] private Transform LB;
    private BoxCollider2D[] rtColliders;
    private BoxCollider2D[] rmColliders;
    private BoxCollider2D[] rbColliders;
    private BoxCollider2D[] ltColliders;
    private BoxCollider2D[] lmColliders;
    private BoxCollider2D[] lbColliders;

    [Header("Damage settings")]
    [SerializeField] private int tentacleDamage = 15;

    private PlayerHealth playerHealth;
    private bool[] tentacleHitPlayer = new bool[6];



    private void Awake()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (animator == null)
            animator = GetComponent<Animator>();

        rtColliders = RT.GetComponentsInChildren<BoxCollider2D>(true);
        rmColliders = RM.GetComponentsInChildren<BoxCollider2D>(true);
        rbColliders = RB.GetComponentsInChildren<BoxCollider2D>(true);
        ltColliders = LT.GetComponentsInChildren<BoxCollider2D>(true);
        lmColliders = LM.GetComponentsInChildren<BoxCollider2D>(true);
        lbColliders = LB.GetComponentsInChildren<BoxCollider2D>(true);

        foreach (BoxCollider2D col in rtColliders)
        {
            col.enabled = false;
        }
        foreach (BoxCollider2D col in rmColliders)
        {
            col.enabled = false;
        }
        foreach (BoxCollider2D col in rbColliders)
        {
            col.enabled = false;
        }
        foreach (BoxCollider2D col in ltColliders)
        {
            col.enabled = false;
        }
        foreach (BoxCollider2D col in lmColliders)
        {
            col.enabled = false;
        }
        foreach (BoxCollider2D col in lbColliders)
        {
            col.enabled = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < tentacleNearby.Length; i++)
        {
            int layer = i + 1;

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(layer);

            // Check if the attack animation is playing.
            if (!state.IsTag("Idle"))
            {
                tentacleAttacking[i] = true;
            }

            // If the tentacle has finished attacking, allow another attack.
            if (tentacleAttacking[i] &&
                state.IsTag("Idle") &&
                !animator.IsInTransition(layer))
            {
                tentacleAttacking[i] = false;

                if (tentacleNearby[i])
                {
                    animator.SetTrigger(GetTriggerName(i));
                }
            }
        }
    }

    private string GetTriggerName(int index)
    {
        string[] triggers = { "RT", "RM", "RB", "LT", "LM", "LB" };
        return triggers[index];
    }

    //Detect where the player's position is, relative to the boss.

    public void SetTentacleNearby(TentacleDetection.TentacleSide side, bool nearby)
    {
        int index = (int)side;

        tentacleNearby[index] = nearby;

        if (nearby && !tentacleAttacking[index])
        {
            animator.SetTrigger(side.ToString());
            tentacleAttacking[index] = true;
        }
        else if (!nearby)
        {
            animator.ResetTrigger(side.ToString());
        }
    }

    public void OnTentacleAttackFinished(string tentacle)
    {
        int index = System.Array.IndexOf(
            new string[] { "RT", "RM", "RB", "LT", "LM", "LB" }, tentacle);

        if (index == -1)
            return;

        tentacleAttacking[index] = false;

        // Attack again if the player is still nearby.
        if (tentacleNearby[index])
        {
            animator.SetTrigger(tentacle);
            tentacleAttacking[index] = true;
        }
    }
    public void EnableTentacleDamage(int index)
    {
        // Enable the corresponding tentacle collider.
    }

    public void DisableTentacleDamage(int index)
    {
        // Disable the corresponding tentacle collider.
    }

    public void TentacleHitPlayer()
    {
        if (tentacleHitPlayer[0])
            return;

        tentacleHitPlayer[0] = true;

        playerHealth.TakeDamage(tentacleDamage);
    }

    //Enabling and disabling tentacle colliders for premade animations (Animation Events
    //Right-Top Tentacle
    public void RTColEnable()
    {
        foreach (BoxCollider2D col in rtColliders)
        {
            tentacleHitPlayer[0] = false;
            col.enabled = true;
        }
    }
    public void RTColDisable()
    {
        foreach (BoxCollider2D col in rtColliders)
        {
            col.enabled = false;
        }
    }

    //Right-Middle Tentacle
    public void RMColEnable()
    {
        foreach (BoxCollider2D col in rmColliders)
        {
            tentacleHitPlayer[0] = false;
            col.enabled = true;
        }
    }
    public void RMColDisable()
    {
        foreach (BoxCollider2D col in rmColliders)
        {
            col.enabled = false;
        }
    }

    //Right-Bottom Tentacle
    public void RBColEnable()
    {
        foreach (BoxCollider2D col in rbColliders)
        {
            tentacleHitPlayer[0] = false;
            col.enabled = true;
        }
    }
    public void RBColDisable()
    {
        foreach (BoxCollider2D col in rbColliders)
        {
            col.enabled = false;
        }
    }

    //Left-Top Tentacle
    public void LTColEnable()
    {
        foreach (BoxCollider2D col in ltColliders)
        {
            tentacleHitPlayer[0] = false;
            col.enabled = true;
        }
    }
    public void LTColDisable()
    {
        foreach (BoxCollider2D col in ltColliders)
        {
            col.enabled = false;
        }
    }

    //Left-Middle Tentacle
    public void LMColEnable()
    {
        foreach (BoxCollider2D col in lmColliders)
        {
            tentacleHitPlayer[0] = false;
            col.enabled = true;
        }
    }
    public void LMColDisable()
    {
        foreach (BoxCollider2D col in lmColliders)
        {
            col.enabled = false;
        }
    }

    //Left-Bottom Tentacle
    public void LBColEnable()
    {
        foreach (BoxCollider2D col in lbColliders)
        {
            tentacleHitPlayer[0] = false;
            col.enabled = true;
        }
    }
    public void LBColDisable()
    {
        foreach (BoxCollider2D col in lbColliders)
        {
            col.enabled = false;
        }
    }

}
