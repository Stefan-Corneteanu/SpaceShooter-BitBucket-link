using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    public AudioSource musicSrc1 = null;
    public AudioSource musicSrc2 = null;
    public AudioSource sfxSrc = null;
    public AudioMixer mixer = null;

    public enum Tracks //TODO: whenever adding a new level/boss, add new tracks
    {
        LEVEL1,
        BOSS1,
        GAMEOVER,
        WON, //1UP is a placeholder clip ill use for this one
        MENU,
        NONE
    }

    public AudioClip[] musicTracks;
    private int activeMusicSrcIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager created");

            //restore prefs if they exist else max volume
            float volMaster = PlayerPrefs.HasKey("VolMaster") ? PlayerPrefs.GetFloat("VolMaster") : 1;
            mixer.SetFloat("VolMaster", Mathf.Log10(volMaster) * 20);

            float volMusic = PlayerPrefs.HasKey("VolMusic") ? PlayerPrefs.GetFloat("VolMusic") : 1;
            mixer.SetFloat("VolMusic", Mathf.Log10(volMusic) * 20);

            float volSFX = PlayerPrefs.HasKey("VolSFX") ? PlayerPrefs.GetFloat("VolSFX") : 1;
            mixer.SetFloat("VolSFX", Mathf.Log10(volSFX) * 20);
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra audio manager");
            Destroy(gameObject);
        }
    }

    public void PlayMusic(Tracks track, bool fade, float fadeDur)
    {
        if (activeMusicSrcIdx == 0 || activeMusicSrcIdx == 2)
        {
            if (!fade)
            {
                mixer.SetFloat("Vol1", Mathf.Log10(1)*20);
                mixer.SetFloat("Vol2", Mathf.Log10(0.0001f));
                musicSrc2.Stop();
            }
            else
            {
                if (activeMusicSrcIdx == 0)
                {
                    mixer.SetFloat("Vol1", Mathf.Log10(0.0001f));
                    mixer.SetFloat("Vol2", Mathf.Log10(0.0001f));
                }
            }
            musicSrc1.clip = musicTracks[(int)track];
            StartCoroutine(DelayedPlayMusic(1));

            if (fade)
            {
                StartCoroutine(Fade(1, fadeDur, 0, 1));
                if (activeMusicSrcIdx == 2)
                {
                    StartCoroutine(Fade(2, fadeDur, 1, 0));
                }
            }
            activeMusicSrcIdx = 1;
        }
        else if (activeMusicSrcIdx == 1)
        {
            if (!fade)
            {
                mixer.SetFloat("Vol1", Mathf.Log10(0.0001f));
                mixer.SetFloat("Vol2", Mathf.Log10(1) * 20);
                musicSrc1.Stop();
            }
            musicSrc2.clip = musicTracks[(int)track];
            StartCoroutine(DelayedPlayMusic(2));
            
            if (fade)
            {
                StartCoroutine(Fade(1, fadeDur, 1, 0));
                StartCoroutine(Fade(2, fadeDur, 0, 1));
            }
            activeMusicSrcIdx = 2;
        }
        
    }
    private IEnumerator DelayedPlayMusic(int srcIdx)
    {
        yield return null;
        if (srcIdx == 1)
        {
            musicSrc1.Play();
        }
        else if(srcIdx == 2)
        {
            musicSrc2.Play();
        }
        else
        {
            Debug.LogError("Invalid source index in AudioManager.DelayedPlayMusic: " + srcIdx);
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSrc.PlayOneShot(clip);
    }

    private IEnumerator Fade(int musicSrcIdx, float dur, int startVol, int tarVol)
    {
        float timer = 0;
        while (timer < dur)
        {
            timer += Time.unscaledDeltaTime;
            float normalizedTime = timer / dur;
            float newVol = Mathf.Lerp(startVol, tarVol, normalizedTime);
            newVol = Mathf.Clamp(newVol, 0.0001f, 0.9999f);
            switch (musicSrcIdx)
            {
                case 1:
                    mixer.SetFloat("Vol1", Mathf.Log10(newVol) * 20);
                    break;
                case 2:
                    mixer.SetFloat("Vol2", Mathf.Log10(newVol) * 20);
                    break;
                default:
                    Debug.LogError("Improper music source used for fading");
                    break;
            }

            yield return null;
        }

        if (tarVol <= 0.0001f) //stop
        {
            switch (musicSrcIdx)
            {
                case 1:
                    musicSrc1.Stop();
                    break;
                case 2:
                    musicSrc2.Stop();
                    break;
                default:
                    Debug.LogError("Improper music source used for fading");
                    break;
            }
        }
        yield return null;
    }

    private IEnumerator FadeInOut(int musicSrcIdx, float durToMid, float durFromMid, int startVol, int midVol, int tarVol)
    {
        StartCoroutine(Fade(musicSrcIdx, durToMid, startVol, midVol));
        yield return new WaitForSeconds(durToMid);
        StartCoroutine(Fade(musicSrcIdx, durFromMid, midVol, tarVol));
        yield return new WaitForSeconds(durFromMid);
    }

    internal void PauseMusic()
    {
        musicSrc1.Pause();
        musicSrc2.Pause();
    }

    internal void ResumeMusic()
    {
        musicSrc1.UnPause();
        musicSrc2.UnPause();
    }
}
