using UnityEngine;

public class CursedBoss : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private bool[] tentacleNearby = new bool[6];
    private bool[] tentacleAttacking = new bool[6];

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
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

}
