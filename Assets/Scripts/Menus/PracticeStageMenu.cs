using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeStageMenu : Menu
{
    public static PracticeStageMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra practice stage menu");
            Destroy(gameObject);
        }
    }

}
