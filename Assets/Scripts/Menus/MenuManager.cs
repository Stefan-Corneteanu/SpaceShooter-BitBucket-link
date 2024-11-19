using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance = null;
    internal Menu activeMenu = null;

    private bool titleMenuShown = false;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("MenuManager created");
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra menu manager");
            Destroy(gameObject);
        }
    }

    public void SwitchToGameplayMenus()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("GraphicsOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("AudioOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ControlsOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("YesNoMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("DebugHUD", LoadSceneMode.Additive);
        SceneManager.LoadScene("GameOverMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("KeypadMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("WellDoneMenu", LoadSceneMode.Additive);
    }

    public void SwitchToMainMenuMenus()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ScoresMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("MedalsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("PlayMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("PracticeMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("PracticeArenaMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("PracticeStageMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("CraftSelectMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("AudioOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ControlsOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("GraphicsOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("YesNoMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ReplaysMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("DebugHUD", LoadSceneMode.Additive);

        if (!titleMenuShown)
        {
            SceneManager.LoadScene("TitleScreenMenu", LoadSceneMode.Additive);
            titleMenuShown = true;
        }
        else
        {
            StartCoroutine(ShowMainMenu());
        }
        
    }

    private IEnumerator ShowMainMenu()
    {
        while (MainMenu.instance == null)
        {
            yield return null;
        }
        MainMenu.instance.TurnOn(null);
    }
}
