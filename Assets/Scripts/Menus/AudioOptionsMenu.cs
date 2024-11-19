using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsMenu : Menu
{
    public static AudioOptionsMenu instance = null;

    public Slider masterVolumeSlider = null;
    public Slider musicVolumeSlider = null;
    public Slider sfxVolumeSlider = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            //restore prefs if they exist else max volume
            float volMaster = PlayerPrefs.HasKey("VolMaster") ? PlayerPrefs.GetFloat("VolMaster") : 1;
            masterVolumeSlider.value = volMaster;

            float volMusic = PlayerPrefs.HasKey("VolMusic") ? PlayerPrefs.GetFloat("VolMusic") : 1;
            musicVolumeSlider.value = volMusic;

            float volSFX = PlayerPrefs.HasKey("VolSFX") ? PlayerPrefs.GetFloat("VolSFX") : 1;
            sfxVolumeSlider.value = volSFX;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra audio options menu");
            Destroy(gameObject);
        }
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

    public void UpdateMasterVol(float val)
    {
        float volume = Mathf.Clamp(val, 0.0001f, 1f);
        AudioManager.instance.mixer.SetFloat("VolMaster", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("VolMaster", volume);
        PlayerPrefs.Save();
    }

    public void UpdateMusicVol(float val)
    {
        float volume = Mathf.Clamp(val, 0.0001f, 1f);
        AudioManager.instance.mixer.SetFloat("VolMusic", Mathf.Log10(volume) * 20 );

        PlayerPrefs.SetFloat("VolMusic", volume);
        PlayerPrefs.Save();
    }

    public void UpdateSFXVol(float val)
    {
        float volume = Mathf.Clamp(val, 0.0001f, 1f);
        AudioManager.instance.mixer.SetFloat("VolSFX", Mathf.Log10(volume) * 20 );

        PlayerPrefs.SetFloat("VolSFX", volume);
        PlayerPrefs.Save();
    }
}
