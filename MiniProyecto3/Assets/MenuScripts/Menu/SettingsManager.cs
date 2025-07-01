using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public TMPro.TMP_Dropdown resolutionsDropdown;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();
        List<Resolution> uniqueResolutions = new List<Resolution>();

        int currentResolutionIndex = 0;

        foreach (var res in resolutions)
        {
            if (!uniqueResolutions.Exists(r => r.width == res.width && r.height == res.height))
            {
                uniqueResolutions.Add(res);
                options.Add(res.width + " x " + res.height);
            }
        }

        resolutions = uniqueResolutions.ToArray();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
