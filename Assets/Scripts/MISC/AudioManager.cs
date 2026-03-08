using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Sound FX")]
    [SerializeField] private AudioClip cannonFireClip;
    [SerializeField] private AudioClip Hit;
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioClip textSFX;
    [SerializeField] private AudioSource backgroundAudioSource;

    private AudioSource audioSource;

    [Header("Volume Settings")]
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;

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
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolumes();
    }

    public void PlayCannon()
    {
        audioSource.pitch = Random.Range(0.75f, 1f);
        audioSource.PlayOneShot(cannonFireClip, 0.2f * sfxVolume);
    }

    public void PlayTextSFX()
    {
        audioSource.pitch = Random.Range(0.6f, 1f);
        audioSource.PlayOneShot(textSFX, 0.2f * sfxVolume);
    }

    public void PlayHit()
    {
        audioSource.pitch = Random.Range(0.5f, 0.7f);
        audioSource.PlayOneShot(Hit, 0.2f * sfxVolume);
    }

    public void PlayExplosion()
    {
        audioSource.pitch = Random.Range(0.5f, 1f);
        audioSource.PlayOneShot(explosion, 0.3f * sfxVolume);
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

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();

        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if(backgroundAudioSource != null)
            backgroundAudioSource.volume = musicVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}
