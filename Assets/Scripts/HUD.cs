using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance = null;
    public AnimatedNumber[] playerScores = new AnimatedNumber[2];
    public AnimatedNumber topScore;
    public GameObject player2Start;
    public PlayerHUD[] playerHUDs = new PlayerHUD[2];
    public GameObject player2HUD;
    private int top;
    public Image fadeScreenImage;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            TurnOnP2(GameManager.instance.player2Exists);
            top = ScoreManager.instance.GetTopScore(GameManager.instance.gameSession.diff);
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra HUD");
            Destroy(gameObject);
            return;
        }
    }

    private void FixedUpdate()
    {
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (!GameManager.instance)
        {
            return;
        }

        //scores
        int p1Score = GameManager.instance.playerDatas[0].score;
        int p2Score = GameManager.instance.playerDatas[1].score;

        //top score
        if (p1Score > top)
        {
            top = p1Score;
        }
        if (p2Score > top)
        {
            top = p2Score;
        }

        topScore.UpdateNumber(top);

        //P1
        //score
        if (playerScores[0])
        {
            
            playerScores[0].UpdateNumber(p1Score);
        }

        //lives, bombs, powers, beam, controls and stats
        UpdateLives(0);
        UpdateBombs(0);
        UpdateChain(0);
        UpdatePower(0);
        UpdateBeam(0);
        UpdateProgress(0);
        UpdateStageScore(0);
        UpdateControls(0);
        UpdateStats(0);

        //P2
        if (GameManager.instance.player2Exists)
        {
            //P2
            if (player2Start)
            {
                player2Start.SetActive(false);
            }
            //score
            if (playerScores[1])
            {
                playerScores[1].UpdateNumber(p2Score);
            }

            //lives, bombs, power, beam, controls and stats
            UpdateLives(1);
            UpdateBombs(1);
            UpdateChain(1);
            UpdatePower(1);
            UpdateBeam(1);
            UpdateProgress(1);
            UpdateStageScore(1);
            UpdateControls(1);
            UpdateStats(1);
        }
        else
        {
            if (player2Start)
            {
                player2Start.SetActive(true);
            }
        }
    }

    internal void TurnOnP2(bool turnOn)
    {
        playerScores[1].gameObject.SetActive(turnOn);
        player2HUD.SetActive(turnOn);
        player2Start.SetActive(!turnOn);
    }

    private void UpdateLives(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerData data = GameManager.instance.playerDatas[playerIdx];
        PlayerHUD hud = playerHUDs[playerIdx];

        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            for (int i = 0; i < hud.lives.Length; i++)
            {
                hud.lives[i].SetActive(false);
            }
            return;
        }

        int lives = data.lives;
        for (int i = 0; i < hud.lives.Length; i++)
        {
            if (lives > i)
            {
                hud.lives[i].SetActive(true);
            }
            else
            {
                hud.lives[i].SetActive(false);
            }
        }
    }

    private void UpdateBombs(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            for (int i = 0; i < hud.bigBombs.Length; i++)
            {
                hud.bigBombs[i].SetActive(false);
            }
            for (int i = 0; i < hud.smallBombs.Length; i++)
            {
                hud.smallBombs[i].SetActive(false);
            }
            return;
        }
        CraftData data = GameManager.instance.gameSession.craftDatas[playerIdx];
        int bigBombs = data.bigBombs;
        int smallBombs = data.smallBombs;
        for (int i = 0; i < hud.bigBombs.Length; i++)
        {
            if (bigBombs > i)
            {
                hud.bigBombs[i].SetActive(true);
            }
            else
            {
                hud.bigBombs[i].SetActive(false);
            }
        }
        for (int i = 0; i < hud.smallBombs.Length; i++)
        {
            if (smallBombs > i)
            {
                hud.smallBombs[i].SetActive(true);
            }
            else
            {
                hud.smallBombs[i].SetActive(false);
            }
        }
    }

    private void UpdatePower(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            for (int i = 0; i < hud.powerMarks.Length; i++)
            {
                hud.powerMarks[i].SetActive(false);
            }
            return;
        }
        CraftData data = GameManager.instance.gameSession.craftDatas[playerIdx];
        int pow = data.shotPower;
        for (int i = 0; i < hud.powerMarks.Length; i++)
        {
            if (pow > i)
            {
                hud.powerMarks[i].SetActive(true);
            }
            else
            {
                hud.powerMarks[i].SetActive(false);
            }   
        }
    }

    private void UpdateBeam(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            for (int i = 0; i < hud.beamMarks.Length; i++)
            {
                hud.beamMarks[i].SetActive(false);
            }
            hud.beamGradient.fillAmount = 0;
            return;
        }
        CraftData data = GameManager.instance.gameSession.craftDatas[playerIdx];
        int pow = data.beamPower;
        for (int i = 0; i < hud.beamMarks.Length; i++)
        {
            if (pow > i)
            {
                hud.beamMarks[i].SetActive(true);
            }
            else
            {
                hud.beamMarks[i].SetActive(false);
            }
        }

        int timer = data.beamTimer;
        hud.beamGradient.fillAmount = (float)timer / (float)Craft.MAX_BEAM_CHARGE;
    }

    private void UpdateControls(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            for (int i = 0; i < hud.buttons.Length; i++)
            {
                hud.buttons[i].SetActive(false);
            }
            hud.up.SetActive(false);
            hud.down.SetActive(false);
            hud.left.SetActive(false);
            hud.right.SetActive(false);
            hud.joystick.SetActive(false);
            return;
        }
        InputState state = InputManager.instance.playerState[playerIdx];

        //buttons
        if (state.shoot)
        {
            hud.buttons[0].SetActive(true);
        }
        else
        {
            hud.buttons[0].SetActive(false);
        }

        if (state.beam)
        {
            hud.buttons[1].SetActive(true);
        }
        else
        {
            hud.buttons[1].SetActive(false);
        }

        if (state.bomb)
        {
            hud.buttons[2].SetActive(true);
        }
        else
        {
            hud.buttons[2].SetActive(false);
        }

        if (state.options)
        {
            hud.buttons[3].SetActive(true);
        }
        else
        {
            hud.buttons[3].SetActive(false);
        }

        //arrows

        if (state.left)
        {
            hud.left.SetActive(true);
        }
        else
        {
            hud.left.SetActive(false);
        }

        if (state.right)
        {
            hud.right.SetActive(true);
        }
        else
        {
            hud.right.SetActive(false);
        }

        if (state.up)
        {
            hud.up.SetActive(true);
        }
        else
        {
            hud.up.SetActive(false);
        }

        if (state.down)
        {
            hud.down.SetActive(true);
        }
        else
        {
            hud.down.SetActive(false);
        }

        //joystick
        hud.joystick.SetActive(true);
        hud.joystick.transform.localPosition = new Vector2(-506.91f, -251.3f) + state.mvmt * 3f;
    }

    private void UpdateStats(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            hud.speedStat.fillAmount = 0;
            hud.powStat.fillAmount = 0;
            hud.bombStat.fillAmount = 0;
            hud.beamStat.fillAmount = 0;
            hud.optionStat.fillAmount = 0;
            return;
        }

        CraftConfig config = GameManager.instance.playerCrafts[playerIdx].config;
        hud.speedStat.fillAmount = (float)config.speed / (float)CraftConfig.MAX_SPEED;
        hud.powStat.fillAmount = (float)config.bulletStr / (float)CraftConfig.MAX_SHOT_POW;
        hud.bombStat.fillAmount = (float)config.bombStr / (float)CraftConfig.MAX_BOMB_POW; ;
        hud.beamStat.fillAmount = (float)config.beamStr / (float)CraftConfig.MAX_BEAM_POW; ;
        hud.optionStat.fillAmount = (float)config.optionStr / (float)CraftConfig.MAX_OPTION_POW; ;
    }

    private void UpdateStageScore(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!GameManager.instance.playerCrafts[playerIdx])
        {
            hud.stageScore.UpdateNumber(0);
            return;
        }
        hud.stageScore.UpdateNumber(GameManager.instance.playerDatas[playerIdx].stageScore);
    }

    private void UpdateProgress(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!(GameManager.instance && GameManager.instance.progressWindow))
        {
            hud.progressBar.fillAmount = 1;
            return;
        }

        float progress = (float)GameManager.instance.progressWindow.data.posY * 44 / (float)GameManager.instance.progressWindow.levelSize;
        hud.progressBar.fillAmount = 1 - progress;
    }

    private void UpdateChain(int playerIdx)
    {
        Debug.Assert(playerIdx < 2 && playerIdx >= 0);
        PlayerHUD hud = playerHUDs[playerIdx];
        if (!(GameManager.instance.playerCrafts[playerIdx]))
        {
            hud.chainScore.UpdateNumber(0);
            hud.chainGradient.fillAmount = 0;
            return;
        }
        hud.chainScore.UpdateNumber(GameManager.instance.playerDatas[playerIdx].chainLevel);
        hud.chainGradient.fillAmount = (float)GameManager.instance.playerDatas[playerIdx].chainTimer / (float)PlayerData.MAX_CHAIN_TIMER;
    }

    public void FadeOutScreen()
    {
        fadeScreenImage.gameObject.SetActive(true);
        fadeScreenImage.color = Color.black;
    }

    public void FadeInScreen()
    {
        fadeScreenImage.gameObject.SetActive(false);
        fadeScreenImage.color = new Color(0, 0, 0, 0);
    }

    [Serializable]
    public class PlayerHUD
    {
        public GameObject[] lives = new GameObject[Craft.MAX_LIVES];

        public GameObject[] bigBombs = new GameObject[Craft.MAX_BIG_BOMBS];
        public GameObject[] smallBombs = new GameObject[Craft.MAX_SMALL_BOMBS];

        public AnimatedNumber chainScore;
        public Image chainGradient;

        public GameObject[] powerMarks = new GameObject[9];

        public GameObject[] beamMarks = new GameObject[5];
        public Image beamGradient;

        public Image progressBar;

        public AnimatedNumber stageScore;

        public GameObject[] buttons = new GameObject[4];
        public GameObject up;
        public GameObject down;
        public GameObject left;
        public GameObject right;
        public GameObject joystick;

        public Image speedStat;
        public Image powStat;
        public Image beamStat;
        public Image optionStat;
        public Image bombStat;
    }
}