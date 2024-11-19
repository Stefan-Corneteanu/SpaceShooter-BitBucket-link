using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public static PauseMenu instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra pause menu");
            Destroy(gameObject);
        }
    }

    public void OnResumeButton()
    {
        GameManager.instance.TogglePause();
    }

    public void OnLoadButton()
    {
        if (SaveManager.instance.LoadExists(1))
        {
            TurnOff(false);
            SaveManager.instance.LoadGame(1);
        }
    }

    public void OnSaveButton()
    {
        SaveManager.instance.SaveGame(1);
    }

    public void OnOptionsButton()
    {
        OptionsMenu.instance.TurnOn(this);
        TurnOff(false);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("MainMenusScene");
    }
}
