using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenMenu : Menu
{
    public static TitleScreenMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra title screen menu");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (root.gameObject.activeInHierarchy)
        {
            if (InputManager.instance.CheckForPlayerInput(0))
            {
                TurnOff(false);
                MainMenu.instance.TurnOn(this);
            }
        }
    }
}
