using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioClip[] sounds;
    
    public void Play()
    {
        if (AudioManager.instance && sounds.Length > 0)
        {
            int i = Random.Range(0, sounds.Length);
            AudioManager.instance.PlaySFX(sounds[i]);
        }
        else
        {
            Debug.LogError("Cannot play SFX, check that there is at least one clip in sounds and that the audio manager is instantiated");
        }
    }
}
