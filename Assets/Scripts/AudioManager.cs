using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SFXVolume";

    [FormerlySerializedAs("music")]
    [SerializeField] private List<Sound> musicLibrary = new();

    [FormerlySerializedAs("sfx")]
    [SerializeField] private List<Sound> sfxLibrary = new();

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private string startupMusicTrack = "Theme";

    private static AudioManager instance;
    private readonly Dictionary<string, AudioClip> musicLookup = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, AudioClip> sfxLookup = new(StringComparer.OrdinalIgnoreCase);

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                var existing = FindObjectOfType<AudioManager>();
                existing?.SetupSingleton();
            }

            return instance;
        }
    }

    public static bool TryGetInstance(out AudioManager manager)
    {
        manager = Instance;
        return manager != null;
    }

    private void Awake()
    {
        SetupSingleton();
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(startupMusicTrack))
        {
            PlayMusic(startupMusicTrack);
        }
    }

    public void PlayMusic(string name)
    {
        if (!musicLookup.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"Music clip '{name}' was not found in the AudioManager library.");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogWarning("Music source is not assigned on AudioManager.");
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        if (!sfxLookup.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX clip '{name}' was not found in the AudioManager library.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("SFX source is not assigned on AudioManager.");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }

        PersistVolume(MusicVolumeKey, volume);
    }

    public void SetSfxVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }

        PersistVolume(SfxVolumeKey, volume);
    }

    public float GetMusicVolume()
    {
        return musicSource != null ? musicSource.volume : PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    }

    public float GetSfxVolume()
    {
        return sfxSource != null ? sfxSource.volume : PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
    }


    private void OnValidate()
    {
        BuildLookups();
    }
    private void SetupSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (instance == this)
        {
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookups();
        ApplySavedVolumes();
    }

    private void BuildLookups()
    {
        musicLookup.Clear();
        sfxLookup.Clear();

        AddToLookup(musicLibrary, musicLookup);
        AddToLookup(sfxLibrary, sfxLookup);
    }

    private static void AddToLookup(IEnumerable<Sound> sounds, IDictionary<string, AudioClip> lookup)
    {
        foreach (var sound in sounds)
        {
            if (sound == null || string.IsNullOrWhiteSpace(sound.Id) || sound.Clip == null)
            {
                continue;
            }

            lookup[sound.Id] = sound.Clip;
        }
    }

    private void ApplySavedVolumes()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(musicVolume);
        }

        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(sfxVolume);
        }
    }

    private static void PersistVolume(string key, float volume)
    {
        PlayerPrefs.SetFloat(key, Mathf.Clamp01(volume));
        PlayerPrefs.Save();
    }
}
