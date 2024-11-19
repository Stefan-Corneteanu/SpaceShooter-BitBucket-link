using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : Menu
{
    public static MainMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra main menu");
            Destroy(gameObject);
        }
    }

    public void OnPlayButton()
    {
        TurnOff(false);
        PlayMenu.instance.TurnOn(this);
    }

    public void OnPracticeButton()
    {
        TurnOff(false);
        PracticeMenu.instance.TurnOn(this);
    }

    public void OnOptionsButton()
    {
        TurnOff(false);
        OptionsMenu.instance.TurnOn(this);
    }

    public void OnScoresButton()
    {
        TurnOff(false);
        ScoresMenu.instance.TurnOn(this);
    }

    public void OnMedalsButton()
    {
        TurnOff(false);
        MedalsMenu.instance.TurnOn(this);
    }

    public void OnReplaysButton()
    {
        TurnOff(false);
        ReplaysMenu.instance.TurnOn(this);
    }

    public void OnCreditsButton()
    {
        TurnOff(false);
        CreditsMenu.instance.TurnOn(this);
    }

    public void OnQuitButton()
    {
        TurnOff(false);
        YesNoMenu.instance.TurnOn(this);
    }

    public void OnLoadButton()
    {
        if (SaveManager.instance.LoadExists(1))
        {
            TurnOff(false);
            SaveManager.instance.LoadGame(1);
        }
    }
}
