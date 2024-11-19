using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerMenu : Menu
{
    public static ControllerMenu instance = null;
    public int player = 0;
    public Text disconnectedText = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra controller menu");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (root.gameObject.activeInHierarchy)
        {
            if (InputManager.instance.CheckForPlayerInput(player))
            {
                TurnOff(false);
                //GameManager.instance.ResumeGameplay();
            }
        }
    }
}
