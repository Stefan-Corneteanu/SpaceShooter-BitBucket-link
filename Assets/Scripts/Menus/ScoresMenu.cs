using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresMenu : Menu
{
    public static ScoresMenu instance = null;
    public Text difficultyText;
    public Text[] scores;
    public Text[] names;
    private Session.Difficulty diff;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            diff = Session.Difficulty.NORMAL; //default to easy difficulty
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra scores menu");
            Destroy(gameObject);
        }
    }

    public void OnNextButton()
    {
        if (diff < Session.Difficulty.INSANE)
        {
            diff++;
        }
        else
        {
            diff = Session.Difficulty.EASY;
        }
        LoadScoresByDiff();
    }

    public void OnPrevButton()
    {
        if (diff > Session.Difficulty.EASY)
        {
            diff--;
        }
        else
        {
            diff = Session.Difficulty.INSANE;
        }
        LoadScoresByDiff();
    }

    public void OnFriendsButton()
    {

    }
    public void OnBackButton()
    {
        TurnOff(true);
    }

    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        LoadScoresByDiff();
        
    }
    private void LoadScoresByDiff()
    {
        switch (diff)
        {
            case Session.Difficulty.EASY:
                difficultyText.text = "Easy";
                break;
            case Session.Difficulty.NORMAL:
                difficultyText.text = "Normal";
                break;
            case Session.Difficulty.HARD:
                difficultyText.text = "Hard";
                break;
            case Session.Difficulty.INSANE:
            default:
                difficultyText.text = "Insane";
                break;
        }
        for (int i = 0; i < 8; i++)
        {
            scores[i].text = ScoreManager.instance.scores[i, (int)diff].ToString();
            names[i].text = ScoreManager.instance.names[i, (int)diff];
        }
    }
}
