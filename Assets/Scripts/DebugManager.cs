using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance = null;
    internal bool displaying = false;
    public GameObject root = null;
    public Toggle invincibiltyToggle = null;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("DebugManager created");
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra debug manager");
            Debug.Assert(root != null, "New debug manager makes root null!");
            Destroy(gameObject);
        }
    }
    public void ToggleHUD()
    {
        if (!displaying) //turn on
        {
            if (!root)
            {
                Debug.LogError("Root Debug Manager not set");
            }
            else
            {
                root.SetActive(true);
                displaying = true;
                Time.timeScale = 0; //pause gameplay
                // Cursor.visible = true;
                PauseMenu.instance.TurnOff(false);
                AudioManager.instance.ResumeMusic();
            }
        }
        else //turn off
        {
            if (!root)
            {
                Debug.LogError("Root Debug Manager not set");
            }
            else
            {
                root.SetActive(false);
                displaying = false;
                Time.timeScale = 1; //resume gameplay
                Cursor.visible = false;
            }
        }
    }

    public void ToggleInvincibility()
    {
        if (invincibiltyToggle)
        {
            GameManager.instance.gameSession.hasInvincibility = invincibiltyToggle.isOn;
        }
        else
        {
            Debug.LogError("Invincibility toggle not set");
        }
    }

    public void OnDeleteScoresFile()
    {
        ScoreManager.instance.DeleteScoresFile();
    }
}
