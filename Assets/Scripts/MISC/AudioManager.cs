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
    [SerializeField] private AudioClip pageTurn;
    [SerializeField] private AudioClip coinPickup;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private Slider backgroundSlider;

    [Header("Music")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private Slider musicSlider;

    private Coroutine musicFadeCoroutine;
    private Coroutine fadeRoutine;

    [Header("Volume Settings")]
    [SerializeField] private float bgVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float fadeSpeed = 1f; // volume units per second
    [SerializeField] private float fadeDelay = 4f; // delay before fading to gameplay music

    private AudioSource audioSource;
    [SerializeField] private AudioSource foghornSource;
    [SerializeField] private PauseManager pauseManager;
    public bool bossActive = false;

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
        foghornSource.PlayOneShot(fogHorn, 0.9f * sfxVolume);
    }

    public void TurnPage()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(pageTurn, 0.2f * sfxVolume);
    }

    public void CoinPickUp()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(coinPickup, 0.2f * sfxVolume);
    }
    #endregion

    #region Music
    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayCombatMusic()
    {
        // Cancel delayed gameplay fade
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        FadeToMusic(combatMusic);
    }

    public void FadeToMusic(AudioClip newClip)
    {
        if (musicAudioSource.clip == newClip) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeMusicRoutine(newClip));
    }

    private IEnumerator FadeMusicRoutine(AudioClip newClip)
    {
        // Fade OUT current music
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        // Switch clip
        musicAudioSource.clip = newClip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();

        // Fade IN new music
        while (musicAudioSource.volume < musicVolume)
        {
            musicAudioSource.volume += fadeSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        musicAudioSource.volume = musicVolume;
    }

    // Fade to gameplay music with delay
    public void PlayGameplayMusic()
    {
        if (musicFadeCoroutine != null)
            StopCoroutine(musicFadeCoroutine);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        musicFadeCoroutine = StartCoroutine(DelayedFadeToGameplay());
    }

    private IEnumerator DelayedFadeToGameplay()
    {
        yield return new WaitForSecondsRealtime(fadeDelay);

        //STOP if boss is active
        if (bossActive)
            yield break;

        FadeToMusic(gameplayMusic);
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