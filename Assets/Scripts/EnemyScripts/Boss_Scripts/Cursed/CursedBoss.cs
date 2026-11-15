using UnityEngine;
using UnityEngine.InputSystem;

public class CursedBoss : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimationTest();
    }

    private void AnimationTest()
    {
        if(Keyboard.current.digit1Key.isPressed)
        {
            animator.SetBool("RT", true);
        }
        if(Keyboard.current.digit2Key.isPressed)
        {
            animator.SetBool("RM", true);
        }
        if(Keyboard.current.digit3Key.isPressed)
        {
            animator.SetBool("RB", true);
        }
        if(Keyboard.current.digit4Key.isPressed)
        {
            animator.SetBool("LT", true);
        }
        if(Keyboard.current.digit5Key.isPressed)
        {
            animator.SetBool("LM", true);
        }
        if(Keyboard.current.digit6Key.isPressed)
        {
            animator.SetBool("LB", true);
        }
        if(Keyboard.current.digit7Key.isPressed)
        {
            animator.SetBool("Slingshot", true);
        }
    }

    public void AnimationEndTest()
    {
        animator.SetBool("RT", false);
        animator.SetBool("RM", false);
        animator.SetBool("RB", false);
        animator.SetBool("LT", false);
        animator.SetBool("LM", false);
        animator.SetBool("LB", false);
        animator.SetBool("Slingshot", false);
    }
}
