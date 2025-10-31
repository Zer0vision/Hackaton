using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        ApplySavedValues();
    }

    private void OnEnable()
    {
        ApplySavedValues();
    }

    public void SetMusicVolume()
    {
        if (!AudioManager.TryGetInstance(out var manager) || musicSlider == null)
        {
            return;
        }

        manager.SetMusicVolume(musicSlider.value);
    }

    public void SetSfxVolume()
    {
        if (!AudioManager.TryGetInstance(out var manager) || sfxSlider == null)
        {
            return;
        }

        manager.SetSfxVolume(sfxSlider.value);
    }

    [Obsolete("Use SetMusicVolume instead.")]
    public void musicVolume()
    {
        SetMusicVolume();
    }

    [Obsolete("Use SetSfxVolume instead.")]
    public void SFXVolume()
    {
        SetSfxVolume();
    }

    private void ApplySavedValues()
    {
        if (musicSlider != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.value = musicVolume;
        }

        if (sfxSlider != null)
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = sfxVolume;
        }

        if (AudioManager.TryGetInstance(out var manager))
        {
            if (musicSlider != null)
            {
                manager.SetMusicVolume(musicSlider.value);
            }

            if (sfxSlider != null)
            {
                manager.SetSfxVolume(sfxSlider.value);
            }
        }
    }
}
