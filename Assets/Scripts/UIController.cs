using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    private AudioManager _audioManager;
    private bool _applyVolumesWhenReady;
    private float _pendingMusicVolume = 1f;
    private float _pendingSfxVolume = 1f;

    private void Awake()
    {
        TryCacheAudioManager();
    }

    private void Start()
    {
        LoadSettings();
    }

    private void Update()
    {
        if (_applyVolumesWhenReady && TryCacheAudioManager())
        {
            ApplyCachedVolumes();
            _applyVolumesWhenReady = false;
        }
    }

    public void musicVolume()
    {
        float value = _musicSlider != null ? _musicSlider.value : _pendingMusicVolume;

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        _pendingMusicVolume = value;

        if (TryCacheAudioManager())
        {
            _audioManager.MusicVolume(value);
        }
        else
        {
            _applyVolumesWhenReady = true;
        }
    }

    public void SFXVolume()
    {
        float value = _sfxSlider != null ? _sfxSlider.value : _pendingSfxVolume;

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        _pendingSfxVolume = value;

        if (TryCacheAudioManager())
        {
            _audioManager.SFXVolume(value);
        }
        else
        {
            _applyVolumesWhenReady = true;
        }
    }

    public void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (_musicSlider != null)
        {
            _musicSlider.SetValueWithoutNotify(musicVolume);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.SetValueWithoutNotify(sfxVolume);
        }

        _pendingMusicVolume = musicVolume;
        _pendingSfxVolume = sfxVolume;

        if (TryCacheAudioManager())
        {
            ApplyCachedVolumes();
        }
        else
        {
            _applyVolumesWhenReady = true;
        }
    }

    private void ApplyCachedVolumes()
    {
        if (_audioManager == null)
        {
            return;
        }

        _audioManager.MusicVolume(_pendingMusicVolume);
        _audioManager.SFXVolume(_pendingSfxVolume);
    }

    private bool TryCacheAudioManager()
    {
        if (_audioManager != null)
        {
            return true;
        }

        if (AudioManager.instance != null)
        {
            _audioManager = AudioManager.instance;
            return true;
        }

        _audioManager = FindObjectOfType<AudioManager>();
        return _audioManager != null;
    }
}
