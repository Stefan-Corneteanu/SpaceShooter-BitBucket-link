using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : Menu
{
    public static OptionsMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra options menu");
            Destroy(gameObject);
        }
    }

    public void OnControllsButton()
    {
        TurnOff(false);
        ControlsOptionsMenu.instance.TurnOn(this);
    }

    public void OnAudioButton()
    {
        TurnOff(false);
        AudioOptionsMenu.instance.TurnOn(this);
    }

    public void OnGraphicsButton()
    {
        TurnOff(false);
        GraphicsOptionsMenu.instance.TurnOn(this);
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }

}