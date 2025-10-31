using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Setings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;
    List<Resolution> filteredResolutions = new List<Resolution>();

    private void Awake()
    {
        LoadSettings();
    }

    private void Start()
    {
        PopulateResolutions();
        LoadUI();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            bool fullscreen = PlayerPrefs.GetInt("FullscreenPreference") == 1;
            Screen.fullScreen = fullscreen;
        }
        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionPreference");
            SetResolutionFromIndex(resIndex);
        }
    }

    private void PopulateResolutions()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        filteredResolutions.Clear();
        HashSet<(int width, int height)> seenResolutions = new HashSet<(int, int)>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var res = resolutions[i];
            var key = (res.width, res.height);
            if (!seenResolutions.Contains(key))
            {
                seenResolutions.Add(key);
                filteredResolutions.Add(res);
                options.Add(res.width + " x " + res.height);
                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SaveResolution(resolutionDropdown.value, isFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Count)
        {
            Resolution res = filteredResolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
        SaveResolution(resolutionIndex, Screen.fullScreen);
    }

    private void SaveResolution(int resIndex, bool fullscreen)
    {
        PlayerPrefs.SetInt("ResolutionPreference", resIndex);
        PlayerPrefs.SetInt("FullscreenPreference", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadUI()
    {
        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            bool fullscreen = PlayerPrefs.GetInt("FullscreenPreference") == 1;
            fullscreenToggle.isOn = fullscreen;
        }
        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionPreference");
            resolutionDropdown.value = resIndex;
        }
        else
        {
            int currentResIndex = -1;
            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                if (filteredResolutions[i].width == Screen.currentResolution.width &&
                    filteredResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                    break;
                }
            }
            if (currentResIndex >= 0)
            {
                resolutionDropdown.value = currentResIndex;
            }
        }
    }

    private void SetResolutionFromIndex(int index)
    {
        if (index >= 0 && index < filteredResolutions.Count)
        {
            Resolution res = filteredResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
    }
}