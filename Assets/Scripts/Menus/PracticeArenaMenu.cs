using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeArenaMenu : Menu
{
    public static PracticeArenaMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra practice arena menu");
            Destroy(gameObject);
        }
    }

}
