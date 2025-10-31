using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    private const string ResolutionPreferenceKey = "ResolutionPreference";
    private const string FullscreenPreferenceKey = "FullscreenPreference";

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private readonly List<Resolution> filteredResolutions = new();

    private void Awake()
    {
        PopulateResolutions();
        LoadSettings();
        SyncUi();
    }

    private void PopulateResolutions()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogWarning("Resolution dropdown is not assigned on SettingsMenu.");
            return;
        }

        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        Resolution[] resolutions = Screen.resolutions;
        var options = new List<string>(resolutions.Length);
        var seenResolutions = new HashSet<(int width, int height)>();

        foreach (var resolution in resolutions)
        {
            var key = (resolution.width, resolution.height);
            if (!seenResolutions.Add(key))
            {
                continue;
            }

            filteredResolutions.Add(resolution);
            options.Add($"{resolution.width} x {resolution.height}");
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SaveDisplaySettings(resolutionDropdown != null ? resolutionDropdown.value : -1, isFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Count)
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            SaveDisplaySettings(resolutionIndex, Screen.fullScreen);
        }
        else
        {
            Debug.LogWarning($"Resolution index {resolutionIndex} is out of range for SettingsMenu.");
        }
    }

    private void LoadSettings()
    {
        bool fullscreen = PlayerPrefs.GetInt(FullscreenPreferenceKey, Screen.fullScreen ? 1 : 0) == 1;
        int resolutionIndex = PlayerPrefs.GetInt(ResolutionPreferenceKey, -1);

        Screen.fullScreen = fullscreen;
        if (resolutionIndex >= 0)
        {
            ApplyResolutionFromIndex(resolutionIndex);
        }
    }

    private void SyncUi()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        if (resolutionDropdown == null)
        {
            return;
        }

        int targetIndex = PlayerPrefs.GetInt(ResolutionPreferenceKey, -1);
        if (targetIndex < 0)
        {
            targetIndex = FindCurrentResolutionIndex();
        }

        if (targetIndex >= 0 && targetIndex < filteredResolutions.Count)
        {
            resolutionDropdown.value = targetIndex;
        }

        resolutionDropdown.RefreshShownValue();
    }

    private int FindCurrentResolutionIndex()
    {
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution resolution = filteredResolutions[i];
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                return i;
            }
        }

        return -1;
    }

    private void ApplyResolutionFromIndex(int index)
    {
        if (index < 0 || index >= filteredResolutions.Count)
        {
            return;
        }

        Resolution resolution = filteredResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SaveDisplaySettings(int resolutionIndex, bool fullscreen)
    {
        if (resolutionIndex >= 0)
        {
            PlayerPrefs.SetInt(ResolutionPreferenceKey, resolutionIndex);
        }

        PlayerPrefs.SetInt(FullscreenPreferenceKey, fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
