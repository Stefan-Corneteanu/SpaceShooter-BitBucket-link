using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeMenu : Menu
{
    public static PracticeMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra practice menu");
            Destroy(gameObject);
        }
    }

    public void OnArenaButton()
    {
        TurnOff(false);
        PracticeArenaMenu.instance.TurnOn(this);
    }

    public void OnStageButton()
    {
        TurnOff(false);
        PracticeStageMenu.instance.TurnOn(this);
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
