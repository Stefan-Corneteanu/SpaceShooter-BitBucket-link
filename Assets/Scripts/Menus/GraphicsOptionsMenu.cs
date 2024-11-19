using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptionsMenu : Menu
{
    public static GraphicsOptionsMenu instance = null;

    public Toggle fullScreenToggle = null;
    private bool fullScreenToApply = true;

    public Text resText = null;
    Resolution resToApply;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            if (fullScreenToggle)
            {
                fullScreenToggle.isOn = ScreenManager.instance.fullScreen;
            }
            else
            {
                Debug.LogError("Error: Did not set full screen toggle");
            }
            fullScreenToApply = ScreenManager.instance.fullScreen;
            resToApply = ScreenManager.instance.crtRes;
            if (resText)
            {
                resText.text = resToApply.width + " X " + resToApply.height + " - " + Mathf.CeilToInt((float)resToApply.refreshRateRatio.value) + "Hz";
            }
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra graphics options menu");
            Destroy(gameObject);
        }
    }

    public void OnPrevButton()
    {
        resToApply = ScreenManager.instance.PrevResolution(resToApply);
        if (resText)
        {
            resText.text = resToApply.width + " X " + resToApply.height + " - " + Mathf.CeilToInt((float)resToApply.refreshRateRatio.value) + "Hz";
        }
    }

    public void OnNextButton()
    {
        resToApply = ScreenManager.instance.NextResolution(resToApply);
        if (resText)
        {
            resText.text = resToApply.width + " X " + resToApply.height + " - " + Mathf.CeilToInt((float)resToApply.refreshRateRatio.value) + "Hz";
        }
    }

    public void OnFullscreenToggle()
    {
        fullScreenToApply = !fullScreenToApply;
    }

    public void OnVSyncButton()
    {

    }

    public void OnApplyButton()
    {
        ScreenManager.instance.fullScreen = fullScreenToApply;
        Screen.fullScreen = fullScreenToApply;

        if (fullScreenToApply)
        {
            Debug.Log("Fullscreen on");
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            Debug.Log("Fullscreen off");
            PlayerPrefs.SetInt("FullScreen", 0);
        }
        PlayerPrefs.SetInt("ScreenWidth", resToApply.width);
        PlayerPrefs.SetInt("ScreenHeight", resToApply.height);
        PlayerPrefs.SetInt("ScreenRate", Mathf.CeilToInt((float)resToApply.refreshRateRatio.value));
        PlayerPrefs.Save();
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
