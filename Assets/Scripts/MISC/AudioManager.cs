using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Slider backgroundSlider;

    [Header("Music")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private Slider musicSlider;

    private Coroutine musicFadeCoroutine;

    [Header("Volume Settings")]
    [SerializeField] private float bgVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float fadeSpeed = 1f; // volume units per second
    [SerializeField] private float fadeDelay = 4f; // delay before fading to gameplay music

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        bgVolume = PlayerPrefs.GetFloat("BackgroundVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");

        ApplyVolumes();
    }

    #region SoundFX
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
    #endregion

    #region Music
    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayCombatMusic() => PlayMusic(combatMusic);

    // Fade to gameplay music with delay
    public void PlayGameplayMusic()
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        musicFadeCoroutine = StartCoroutine(FadeToGameplayWithDelay());
    }

    private IEnumerator FadeToGameplayWithDelay()
    {
        // Wait before starting fade
        yield return new WaitForSecondsRealtime(fadeDelay);

        // Fade out
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        // Switch clip
        musicAudioSource.clip = gameplayMusic;
        musicAudioSource.loop = true;
        musicAudioSource.Play();

        // Fade in
        while (musicAudioSource.volume < musicVolume)
        {
            musicAudioSource.volume += fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        musicAudioSource.volume = musicVolume;
        musicFadeCoroutine = null;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicAudioSource.clip == clip) return;

        musicAudioSource.clip = clip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
        musicAudioSource.volume = musicVolume;
    }
    #endregion

    #region Background Controls
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
        volume = backgroundSlider.value;
        bgVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("BackgroundVolume", bgVolume);
        PlayerPrefs.Save();

        ApplyVolumes();
    }
    #endregion

    #region Volume Setters
    public void SetMusicVolume(float volume)
    {
        volume = musicSlider.value;
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();

        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
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

    public float GetMusicVolume()
    {
        return musicVolume;
    }
    public float GetBackgroundVolume() => bgVolume;
    public float GetSFXVolume() => sfxVolume;
    #endregion
}