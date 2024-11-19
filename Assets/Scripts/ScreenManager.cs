using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance = null;
    public bool fullScreen = true;
    public Resolution crtRes;
    public Resolution[] allRes;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            crtRes = Screen.currentResolution;
            allRes = Screen.resolutions;
            RestoreSettings();
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra ScreenManager");
            Destroy(gameObject);
        }
    }

    public void SetResolution(Resolution res)
    {
        if (fullScreen)
        {
            Screen.SetResolution(res.width, res.height, FullScreenMode.ExclusiveFullScreen, res.refreshRateRatio);
        }
        else
        {
            Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed, res.refreshRateRatio);
        }
        PlayerPrefs.SetInt("ScreenWidth", res.width);
        PlayerPrefs.SetInt("ScreenHeight", res.height);
        PlayerPrefs.SetInt("ScreenRate", Mathf.CeilToInt((float)res.refreshRateRatio.value));

        //Cursor.visible = false;
    }

    void RestoreSettings()
    {
        //Restore resolution (default to FHD (1920 X 1080 - 60 Hz))
        int width = PlayerPrefs.HasKey("ScreenWidth") ? PlayerPrefs.GetInt("ScreenWidth") : 1920;
        int height = PlayerPrefs.HasKey("ScreenHeight") ? PlayerPrefs.GetInt("ScreenHeight") : 1080;
        int rate = PlayerPrefs.HasKey("ScreenRate") ? PlayerPrefs.GetInt("ScreenRate") : 60;
        crtRes = FindResolution(width, height, rate);
        SetResolution(crtRes);

        //Restore fullscreen
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            int fullScreenVal = PlayerPrefs.GetInt("FullScreen");
            switch (fullScreenVal)
            {
                case 0:
                    fullScreen = false;
                    break;
                case 1:
                    fullScreen = true;
                    break;
                default:
                    Debug.LogError("Invalid FullScreen value in ScreenManager.RestoreSettings");
                    break;
            }
        }
        else
        {
            fullScreen = true;
        }
        Screen.fullScreen = fullScreen;
    }
    private Resolution FindResolution(int width, int height, int rate)
    {
        foreach (Resolution res in allRes){
            if (res.width == width && res.height == height && Mathf.CeilToInt((float)res.refreshRateRatio.value) == rate)
            {
                return res;
            }
        }
        Debug.LogError(width + "X" + height + " - " + rate + " Hz not found");
        return crtRes;
    }
    internal Resolution NextResolution(Resolution crtRes)
    {
        int idx = FindResolutionIdx(crtRes);
        if (idx == -1)
        {
            return crtRes;
        }
        else if (idx == allRes.Length - 1)
        {
            return allRes[0];
        }
        else
        {
            return allRes[idx + 1];
        }
    }

    internal Resolution PrevResolution(Resolution crtRes)
    {
        int idx = FindResolutionIdx(crtRes);
        if (idx == -1)
        {
            return crtRes;
        }
        else if (idx == 0)
        {
            return allRes[allRes.Length - 1];
        }
        else
        {
            return allRes[idx - 1];
        }
    }

    private int FindResolutionIdx(Resolution res)
    {
        for (int i = 0; i < allRes.Length; i++)
        {
            if (res.width == allRes[i].width && res.height == allRes[i].height 
            && Mathf.CeilToInt((float)res.refreshRateRatio.value) == Mathf.CeilToInt((float)allRes[i].refreshRateRatio.value))
            {
                return i;
            }
        }
        Debug.LogError(res.width + "X" + res.height + " - " + Mathf.CeilToInt((float)res.refreshRateRatio.value) + " Hz not found");
        return -1;
    }
}
