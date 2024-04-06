using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TextMeshProUGUI crntVolumeText;


    Resolution[] resolutions;
    public TMP_Dropdown resDropdown;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> resStr = new List<string>();

        int crntRes = 0;

        for (int i = 0; i < resolutions.Length; i++) 
        { 
            string res = resolutions[i].width + " x " + resolutions[i].height;
            resStr.Add(res);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                crntRes = i;
            }
        }
        resDropdown.AddOptions(resStr);
        resDropdown.value = crntRes;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        crntVolumeText.text = (volume + 80).ToString();
    }
    public void SetScreenMode(int scrnMode)
    {
        switch (scrnMode)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break; 
            case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
    public void SetResolution(int res)
    {
        Screen.SetResolution(resolutions[res].width, resolutions[res].width,Screen.fullScreen);
    }
}
