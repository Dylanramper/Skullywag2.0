using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Sound FX")]
    [SerializeField] private AudioClip cannonFireClip;
    [SerializeField] private AudioClip Hit;
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioSource backgroundAudioSource;

    private AudioSource audioSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCannon()
    {
        audioSource.pitch = Random.Range(0.75f, 1f);
        audioSource.PlayOneShot(cannonFireClip, 0.2f);
    }

    public void PlayHit()
    {
        audioSource.pitch = Random.Range(0.5f, 0.7f);
        audioSource.PlayOneShot(Hit, 0.2f);
    }

    public void PlayExplosion()
    {
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.PlayOneShot(explosion, 0.3f);
    }

    public void PauseBackground()
    {
        if (backgroundAudioSource != null && backgroundAudioSource.isPlaying)
            backgroundAudioSource.Pause();
    }

    public void ResumeBackground()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
            backgroundAudioSource.UnPause();
    }
}
