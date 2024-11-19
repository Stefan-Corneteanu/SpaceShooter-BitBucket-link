using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeypadMenu : Menu
{
    public static KeypadMenu instance = null;
    public Text enterText = null;
    public new Text name = null;
    public int playerIdx = 0;
    public bool bothPlayers = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra keypad menu");
            Destroy(gameObject);
            return;
        }
    }

    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        enterText.text = "Enter your name player " + playerIdx + 1 + ":";
    }

    public void OnEnterButton()
    {
        ScoreManager.instance.AddScore(GameManager.instance.playerDatas[playerIdx].score, 
            GameManager.instance.gameSession.diff, name.text);
        if (bothPlayers && playerIdx == 0)
        {
            playerIdx = 1;
            enterText.text = "Enter your name player " + playerIdx + 1 + ":";
            name.text = "PLAYER";
        }
        else
        {
            ScoreManager.instance.SaveScores();
            TurnOff(false);
            SceneManager.LoadScene("MainMenusScene");
        }
    }

    public void OnClearButton()
    {
        name.text = "";
    }

    public void OnDeleteButton()
    {
        if (name.text.Length > 0)
        {
            name.text = name.text.Substring(0, name.text.Length - 1);
        }
    }

    public void OnKeyPress(int key)
    {
        name.text += (char)key;
    }
}
