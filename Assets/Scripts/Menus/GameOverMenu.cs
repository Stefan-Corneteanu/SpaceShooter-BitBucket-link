using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : Menu
{
    public static GameOverMenu instance = null;

    public Text score = null;
    public Text hiScore = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra game over menu");
            Destroy(gameObject);
        }
    }

    public void OnContinueButton()
    {
        TurnOff(false);
        int p1Score = GameManager.instance.playerDatas[0].score;
        int p2Score = GameManager.instance.playerDatas[1].score;
        Session.Difficulty diff = GameManager.instance.gameSession.diff;
        bool isP1HiScore = ScoreManager.instance.IsHiScore(p1Score, diff);
        bool isP2HiScore = ScoreManager.instance.IsHiScore(p2Score, diff);
        if (isP1HiScore || isP2HiScore)
        {
            if (GameManager.instance.player2Exists)
            {
                bool isP1ScoreNotLastHiScore = ScoreManager.instance.IsScoreHigherThan2ndLowestHighScore(p1Score, diff);
                bool isP2ScoreNotLastHiScore = ScoreManager.instance.IsScoreHigherThan2ndLowestHighScore(p2Score, diff);
                //CORNER CASE! BOTH PLAYERS HAVE HIGH SCORE BUT ONE OF THEM IS ONLY ABOVE THE SMALLEST!
                //SHOULD GIVE KEYPAD MENU ONLY TO HIGHEST SCORIN PLAYER
                if (isP1ScoreNotLastHiScore && isP2ScoreNotLastHiScore)
                {
                    KeypadMenu.instance.bothPlayers = true;
                    KeypadMenu.instance.playerIdx = 0;
                }
                else
                {
                    KeypadMenu.instance.bothPlayers = false;
                    KeypadMenu.instance.playerIdx = p1Score > p2Score ? 0 : 1;
                    // at least one player has a high score but not both of them have a score higher than second last?
                    // select the highest score
                }
            }
            else
            {
                KeypadMenu.instance.bothPlayers = false;
                KeypadMenu.instance.playerIdx = 0;
            }
            KeypadMenu.instance.TurnOn(null);
        }
        else
        {
            SceneManager.LoadScene("MainMenusScene");
        }
    }

    internal void GameOver()
    {
        TurnOn(null);
        AudioManager.instance.PlayMusic(AudioManager.Tracks.GAMEOVER, true, 0.5f);
        score.text = "P1: " + GameManager.instance.playerDatas[0].score.ToString();
        if (GameManager.instance.player2Exists)
        {
            score.text += "\nP2: " + GameManager.instance.playerDatas[1].score.ToString();
        }

        if (ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[0].score, GameManager.instance.gameSession.diff)
        || ScoreManager.instance.IsHiScore(GameManager.instance.playerDatas[1].score, GameManager.instance.gameSession.diff))
        {
            hiScore.gameObject.SetActive(true);
        }
        else
        {
            hiScore.gameObject.SetActive(false);
        }
    }
}
