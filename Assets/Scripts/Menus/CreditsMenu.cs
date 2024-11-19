using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : Menu
{
    public static CreditsMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra credits menu");
            Destroy(gameObject);
        }
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
