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
        PopulateResolutions();
        LoadSettings();
    }

    private void Start()
    {
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
            if (!TrySetResolutionFromIndex(resIndex) && PlayerPrefs.HasKey("ResolutionPreference"))
            {
                PlayerPrefs.DeleteKey("ResolutionPreference");
                PlayerPrefs.Save();
            }
        }
    }

    private void PopulateResolutions()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        filteredResolutions.Clear();
        HashSet<(int width, int height)> seenResolutions = new HashSet<(int, int)>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            var res = resolutions[i];
            var key = (res.width, res.height);
            if (!seenResolutions.Contains(key))
            {
                seenResolutions.Add(key);
                filteredResolutions.Add(res);
                options.Add(res.width + " x " + res.height);
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
        if (TrySetResolutionFromIndex(resolutionIndex))
        {
            SaveResolution(resolutionIndex, Screen.fullScreen);
        }
    }

    private void SaveResolution(int resIndex, bool fullscreen)
    {
        if (IsValidResolutionIndex(resIndex))
        {
            PlayerPrefs.SetInt("ResolutionPreference", resIndex);
        }
        else if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            PlayerPrefs.DeleteKey("ResolutionPreference");
        }
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
        bool appliedSavedResolution = false;

        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionPreference");
            if (IsValidResolutionIndex(resIndex))
            {
                resolutionDropdown.value = resIndex;
                appliedSavedResolution = true;
            }
        }

        if (!appliedSavedResolution)
        {
            SetDropdownToCurrentResolution();
        }
        resolutionDropdown.RefreshShownValue();
    }

    private bool TrySetResolutionFromIndex(int index)
    {
        if (IsValidResolutionIndex(index))
        {
            Resolution res = filteredResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            return true;
        }

        return false;
    }

    private bool IsValidResolutionIndex(int index)
    {
        return index >= 0 && index < filteredResolutions.Count;
    }

    private void SetDropdownToCurrentResolution()
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
