using UnityEngine;

public class NapalmFlameFX : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartFiring()
    {
        gameObject.SetActive(true);
        anim.Play("Napalm_Anim");
    }
    public void StopFiring()
    {
        gameObject.SetActive(false);
        //anim.Play("FlameEnd");
    }
}
