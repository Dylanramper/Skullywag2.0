using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Sound FX")]
    [SerializeField] private AudioClip cannonFireClip;
    [SerializeField] private AudioClip Hit;
    [SerializeField] private AudioClip explosion;
    [SerializeField] private AudioClip textSFX;
    [SerializeField] private AudioClip fogHorn;
    [SerializeField] private AudioSource backgroundAudioSource;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip combatMusic;

    private AudioSource audioSource;
    [SerializeField] private AudioSource musicAudioSource;

    [Header("Volume Settings")]
    [SerializeField] private float bgVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 1f;

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
        bgVolume = PlayerPrefs.GetFloat("BackgroundVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

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

    public void PlayFogHorn()
    {
        audioSource.PlayOneShot(fogHorn, 0.2f * sfxVolume);
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

    public void SetBackgroundVolume(float volume)
    {
        bgVolume = volume;
        PlayerPrefs.SetFloat("BackgroundVolume", bgVolume);
        PlayerPrefs.Save();

        ApplyVolumes();
    }

    public void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicAudioSource.clip == clip) return;

        musicAudioSource.clip = clip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
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
        if (backgroundAudioSource != null)
            backgroundAudioSource.volume = bgVolume;

        if (musicAudioSource != null)
            musicAudioSource.volume = musicVolume;
    }
    public float GetBackgroundVolume()
    {
        return bgVolume;
    }
    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}
