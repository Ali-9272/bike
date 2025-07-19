using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip gameOverMusic;
    
    [Header("Sound Effect Clips")]
    public AudioClip engineSound;
    public AudioClip wheelieSound;
    public AudioClip crashSound;
    public AudioClip buttonClickSound;
    public AudioClip scoreSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    
    private static AudioManager instance;
    
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AudioManager>();
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadAudioSettings();
        PlayMainMenuMusic();
    }
    
    public void PlayMainMenuMusic()
    {
        if (mainMenuMusic != null && musicSource != null)
        {
            musicSource.clip = mainMenuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null && musicSource != null)
        {
            musicSource.clip = gameplayMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null && musicSource != null)
        {
            musicSource.clip = gameOverMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }
    
    public void PlayEngineSound()
    {
        PlaySFX(engineSound);
    }
    
    public void PlayWheelieSound()
    {
        PlaySFX(wheelieSound);
    }
    
    public void PlayCrashSound()
    {
        PlaySFX(crashSound);
    }
    
    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }
    
    public void PlayScoreSound()
    {
        PlaySFX(scoreSound);
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
        SaveAudioSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
        SaveAudioSettings();
    }
    
    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicSource.mute;
            SaveAudioSettings();
        }
    }
    
    public void ToggleSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.mute = !sfxSource.mute;
            SaveAudioSettings();
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    public void StopAllSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();
    }
    
    private void LoadAudioSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.mute = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        }
        
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.mute = PlayerPrefs.GetInt("SFXMuted", 0) == 1;
        }
    }
    
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        
        if (musicSource != null)
            PlayerPrefs.SetInt("MusicMuted", musicSource.mute ? 1 : 0);
        
        if (sfxSource != null)
            PlayerPrefs.SetInt("SFXMuted", sfxSource.mute ? 1 : 0);
        
        PlayerPrefs.Save();
    }
}