using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI Sliders (optional)")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Test SFX")]
    public AudioClip testSFX; // Assign a short sound cue in Inspector

    private static AudioManager instance;

    private const string MusicPref = "MusicVolume";
    private const string SFXPref = "SFXVolume";

    void Awake()
    {
        // Keep only one instance across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Load saved volume levels
        float musicVolume = PlayerPrefs.GetFloat(MusicPref, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXPref, 1f);

        ApplyVolume(musicVolume, sfxVolume);

        // Connect sliders if they exist in this scene
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat(MusicPref, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(SFXPref, value);
        PlayerPrefs.Save();

        // Play a quick test sound to preview volume
        if (testSFX != null && !sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(testSFX, value);
        }
    }

    private void ApplyVolume(float music, float sfx)
    {
        if (musicSource != null) musicSource.volume = music;
        if (sfxSource != null) sfxSource.volume = sfx;
    }
}
