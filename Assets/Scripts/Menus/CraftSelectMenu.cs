using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CraftSelectMenu : Menu
{
    public static CraftSelectMenu instance = null;

    public Image P1CraftA = null;
    public Image P1CraftB = null;
    public Image P1CraftC = null;
    public Image P1CraftX = null;
    public Image P1CraftY = null;

    public Image P2CraftA = null;
    public Image P2CraftB = null;
    public Image P2CraftC = null;
    public Image P2CraftX = null;
    public Image P2CraftY = null;

    public Slider P1PowSlider = null;
    public Slider P1SpeedSlider = null;
    public Slider P1BeamSlider = null;
    public Slider P1BombSlider = null;
    public Slider P1OptionsSlider = null;

    public Slider P2PowSlider = null;
    public Slider P2SpeedSlider = null;
    public Slider P2BeamSlider = null;
    public Slider P2BombSlider = null;
    public Slider P2OptionsSlider = null;

    public Text countdownText = null;
    public GameObject P2Panel = null;
    public Text P2StartText = null;

    private float lastUnscaledTime = 0;
    private float timer = 5.9f;
    private bool countdown = false;

    public Sprite[] stdSprites = new Sprite[5];
    public Sprite[] selectedSprites = new Sprite[5];
    public Sprite[] disabledSprites = new Sprite[5];

    public CraftConfig[] craftConfigs = new CraftConfig[5];

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra craft select menu");
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (InputManager.instance.playerState[0].shoot)
        {
            StartCountdown();
        }

        if (InputManager.instance.playerState[1].shoot && !GameManager.instance.player2Exists)
        {
            StopCountdown();
            GameManager.instance.player2Exists = true;
            P2StartText.gameObject.SetActive(false);
            P2Panel.SetActive(true);
            UpdateShipSelected(1);
            HUD.instance.TurnOnP2(true);
        }

        if (GameManager.instance && InputManager.instance.playerState[0].left && !InputManager.instance.prevPlayerState[0].left)
        {
            if (GameManager.instance.gameSession.P1SelectedShip > 0)
            {
                GameManager.instance.gameSession.P1SelectedShip--;
            }
            else
            {
                GameManager.instance.gameSession.P1SelectedShip = 4;
            }
            UpdateShipSelected(0);
        }

        if (GameManager.instance && InputManager.instance.playerState[0].right && !InputManager.instance.prevPlayerState[0].right)
        {
            if (GameManager.instance.gameSession.P1SelectedShip < 4)
            {
                GameManager.instance.gameSession.P1SelectedShip++;
            }
            else
            {
                GameManager.instance.gameSession.P1SelectedShip = 0;
            }
            UpdateShipSelected(0);
        }

        if (GameManager.instance && InputManager.instance.playerState[1].left && !InputManager.instance.prevPlayerState[1].left)
        {
            if (GameManager.instance.gameSession.P2SelectedShip > 0)
            {
                GameManager.instance.gameSession.P2SelectedShip--;
            }
            else
            {
                GameManager.instance.gameSession.P2SelectedShip = 4;
            }
            UpdateShipSelected(1);
        }

        if (GameManager.instance && InputManager.instance.playerState[1].right && !InputManager.instance.prevPlayerState[1].right)
        {
            if (GameManager.instance.gameSession.P2SelectedShip < 4)
            {
                GameManager.instance.gameSession.P2SelectedShip++;
            }
            else
            {
                GameManager.instance.gameSession.P2SelectedShip = 0;
            }
            UpdateShipSelected(1);
        }

        if (countdown)
        {
            float dUnscaled = Time.unscaledTime - lastUnscaledTime;
            lastUnscaledTime = Time.unscaledTime;
            timer -= dUnscaled;
            countdownText.text = ((int)timer).ToString();
            if (timer < 1)
            {
                TurnOff(false);
                GameManager.instance.StartGame();
            }
        }
    }

    private void Reset()
    {
        //Reset to single player
        P2StartText.gameObject.SetActive(true);
        P2Panel.SetActive(false);
        GameManager.instance.player2Exists = false;

        StopCountdown();
        timer = 5.9f;
        UpdateShipSelected(0);
        if (GameManager.instance.player2Exists)
        {
            UpdateShipSelected(1);
        }
    }

    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        Reset();
    }
    public void OnPlayButton()
    {
        StartCountdown();
    }

    private void StartCountdown()
    {
        timer = 5.9f;
        lastUnscaledTime = Time.unscaledTime;
        countdownText.gameObject.SetActive(true);
        countdown = true;
    }

    private void StopCountdown()
    {
        countdownText.gameObject.SetActive(false);
        countdown = false;
    }

    //DUPLICATE CODE CAUSED BY UNITY LIMITATIONS!
    public void OnCraftSelectButtonP1(int craftIdx)
    {
        GameManager.instance.gameSession.P1SelectedShip = (byte)craftIdx;
        UpdateShipSelected(0);
    }

    public void OnCraftSelectButtonP2(int craftIdx)
    {
        GameManager.instance.gameSession.P2SelectedShip = (byte)craftIdx;
        UpdateShipSelected(1);
    }

    public void OnBackButton()
    {
        StopCountdown();
        TurnOff(true);
    }
    private void UpdateShipSelected(byte playerIdx)
    {
        CraftConfig craftConfig;
        switch (playerIdx)
        {
            case 0:
                P1CraftA.sprite = stdSprites[0];
                P1CraftB.sprite = stdSprites[1];
                P1CraftC.sprite = stdSprites[2];
                P1CraftX.sprite = disabledSprites[3];
                P1CraftY.sprite = disabledSprites[4];

                switch (GameManager.instance.gameSession.P1SelectedShip)
                {
                    case 0:
                        P1CraftA.sprite = selectedSprites[0];
                        break;
                    case 1:
                        P1CraftB.sprite = selectedSprites[1];
                        break;
                    case 2:
                        P1CraftC.sprite = selectedSprites[2];
                        break;
                    case 3:
                        P1CraftX.sprite = selectedSprites[3];
                        break;
                    case 4:
                        P1CraftY.sprite = selectedSprites[4];
                        break;
                }

                craftConfig = craftConfigs[GameManager.instance.gameSession.P1SelectedShip];
                P1PowSlider.value = craftConfig.bulletStr;
                P1SpeedSlider.value = craftConfig.speed;
                P1BeamSlider.value = craftConfig.beamStr;
                P1BombSlider.value = craftConfig.bombStr;
                P1OptionsSlider.value = craftConfig.optionStr;

                break;
            case 1:
                if (!GameManager.instance.player2Exists)
                {
                    Debug.LogError("Trying to update player 2 craft while it does not exist");
                    return;
                }
                P2CraftA.sprite = stdSprites[0];
                P2CraftB.sprite = stdSprites[1];
                P2CraftC.sprite = stdSprites[2];
                P2CraftX.sprite = disabledSprites[3];
                P2CraftY.sprite = disabledSprites[4];

                switch (GameManager.instance.gameSession.P2SelectedShip)
                {
                    case 0:
                        P2CraftA.sprite = selectedSprites[0];
                        break;
                    case 1:
                        P2CraftB.sprite = selectedSprites[1];
                        break;
                    case 2:
                        P2CraftC.sprite = selectedSprites[2];
                        break;
                    case 3:
                        P2CraftX.sprite = selectedSprites[3];
                        break;
                    case 4:
                        P2CraftY.sprite = selectedSprites[4];
                        break;
                }

                craftConfig = craftConfigs[GameManager.instance.gameSession.P2SelectedShip];
                P2PowSlider.value = craftConfig.bulletStr;
                P2SpeedSlider.value = craftConfig.speed;
                P2BeamSlider.value = craftConfig.beamStr;
                P2BombSlider.value = craftConfig.bombStr;
                P2OptionsSlider.value = craftConfig.optionStr;

                break;
            default:
                Debug.LogError("Passed an invalid player index in ship selection update: " + playerIdx);
                break;
        }
    }
    
}
