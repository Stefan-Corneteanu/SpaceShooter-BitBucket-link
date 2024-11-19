using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenu : Menu
{
    public static PlayMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra play menu");
            Destroy(gameObject);
        }
    }

    public void OnDifficultyButton(int diff)
    {
        GameManager.instance.gameSession.diff = (Session.Difficulty)diff;
        TurnOff(false);
        CraftSelectMenu.instance.TurnOn(this);
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
