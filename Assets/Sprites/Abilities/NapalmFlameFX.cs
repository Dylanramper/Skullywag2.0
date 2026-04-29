using UnityEngine;

public class NapalmFlameFX : MonoBehaviour
{
    private Animator anim;

    private bool isFiring;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartFiring()
    {
        if (isFiring) return;

        isFiring = true;
        gameObject.SetActive(true);

        anim.Play("Napalm_Start");
    }
    public void StopFiring()
    {
        if (!isFiring) return;

        isFiring = false;

        anim.SetTrigger("Stop");
    }

    public void OnFlameEndFinished()
    {
        gameObject.SetActive(false);
    }
}
