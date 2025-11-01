using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    private AudioManager _audioManager;

    private void Awake()
    {
        TryCacheAudioManager();
    }

    private void Start()
    {
        LoadSettings();
    }

    public void musicVolume()
    {
        if (!TryCacheAudioManager())
        {
            return;
        }

        _audioManager.MusicVolume(_musicSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.Save();
    }

    public void SFXVolume()
    {
        if (!TryCacheAudioManager())
        {
            return;
        }

        _audioManager.SFXVolume(_sfxSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _sfxSlider.value);
        PlayerPrefs.Save();
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

        if (TryCacheAudioManager())
        {
            _audioManager.MusicVolume(musicVolume);
            _audioManager.SFXVolume(sfxVolume);
        }
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
        if (_audioManager == null)
        {
            Debug.LogWarning("AudioManager instance not found when accessing UIController.");
            return false;
        }

        return true;
    }
}
