using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaysMenu : Menu
{
    public static ReplaysMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra medals menu");
            Destroy(gameObject);
        }
    }

    public void OnBackButton()
    {
        TurnOff(true);
    }
}
