using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public bool player2Exists = false;
    public GameObject[] craftPrefabs;
    public Craft[] playerCrafts = new Craft[2];
    public PlayerData[] playerDatas;

    public BulletManager bulletManager = null;

    public LevelProgress progressWindow = null;

    public Session gameSession = new Session();

    public Pickup[] cyclicDrops = new Pickup[15];
    public Pickup[] medals = new Pickup[10];
    private int crtDropIdx = 0;
    private int crtMedalIdx = 0;

    public Pickup option = null;
    public Pickup powerup = null;
    public Pickup beamup = null;
    public enum GameState
    {
        INVALID,
        INMENUS,
        PLAYING,
        PAUSED
    }
    public GameState gameState = GameState.INVALID;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager created");
            bulletManager = GetComponent<BulletManager>();
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra game manager");
            Destroy(gameObject);
        }

        playerDatas = new PlayerData[2];
        playerDatas[0] = new PlayerData();
        playerDatas[1] = new PlayerData();

        Application.targetFrameRate = 60;
    }

    public void SpawnPlayer(byte playerIdx, int craftType)
    {
        Debug.Assert(craftType < craftPrefabs.Length);
        Debug.Log("Spawning player"+playerIdx);
        playerCrafts[playerIdx] = Instantiate(craftPrefabs[craftType]).GetComponent<Craft>();
        playerCrafts[playerIdx].SetPlayerIdx(playerIdx);
        playerCrafts[playerIdx].isAlive = true;

        if (player2Exists)
        {
            if (playerIdx == 0)
            {
                gameSession.craftDatas[playerIdx].posX -= 2f;
            }
            else
            {
                gameSession.craftDatas[playerIdx].posX += 2f;
            }
        }
    }

    public void SpawnPlayers()
    {
        SpawnPlayer(0, gameSession.P1SelectedShip); 
        if (player2Exists)
        {
            SpawnPlayer(1, gameSession.P2SelectedShip);
        }
    }
    internal void DelayedRespawn(byte playerIdx)
    {
        StartCoroutine(RespawnCoroutine(playerIdx));
    }

    IEnumerator RespawnCoroutine(byte playerIdx)
    {
        yield return new WaitForSeconds(1.5f);
        if (playerIdx == 0)
        {
            SpawnPlayer(playerIdx, gameSession.P1SelectedShip);
        }
        else
        {
            SpawnPlayer(playerIdx, gameSession.P2SelectedShip);
        }
        yield return null;
    }

    public void RestoreState(byte playerIdx)
    {
        int noOptions = gameSession.craftDatas[playerIdx].noEnabledOptions;
        gameSession.craftDatas[playerIdx].noEnabledOptions = 0;
        gameSession.craftDatas[playerIdx].posX = 0;
        gameSession.craftDatas[playerIdx].posY = 0;

        for (int i = 0; i < noOptions; i++)
        {
            playerCrafts[playerIdx].AddOption(0);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugManager.instance.ToggleHUD();
        }
    }

    public void StartGame()
    {
        gameState = GameState.PLAYING;
        gameSession.craftDatas[0].ResetData();
        gameSession.craftDatas[1].ResetData();
        playerDatas[0].ResetData();
        playerDatas[1].ResetData();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1");
    }

    internal void PickupFallOffScreen(Pickup pickup)
    {
        if (pickup.config.type == Pickup.PickupType.MEDAL)
        {
            crtMedalIdx = 0;
        }
    }

    public Pickup GetNextDrop()
    {
        Pickup result = cyclicDrops[crtDropIdx];
        crtDropIdx++;

        if (result.config.type == Pickup.PickupType.MEDAL)
        {
            result = medals[crtMedalIdx];
            crtMedalIdx++;
            if (crtMedalIdx >= medals.Length)
            {
                crtMedalIdx = 0;
            }
        }

        if (crtDropIdx >= cyclicDrops.Length)
        {
            crtDropIdx = 0;
        }
        return result;
    }

    public Pickup SpawnPickup(Pickup pickupPrefab, Vector2 pos)
    {
        Pickup spawn = Instantiate(pickupPrefab, pos, Quaternion.identity);
        if (spawn)
        {
            spawn.transform.SetParent(GameManager.instance.transform);
        }
        else
        {
            Debug.LogError("Failed to spawn pickup");
        }
        return spawn;
    }

    internal void TogglePause()
    {
        if (gameState == GameState.PLAYING || DebugManager.instance.displaying) //pause the game
        {
            gameState = GameState.PAUSED;
            AudioManager.instance.PauseMusic();
            PauseMenu.instance.TurnOn(null);
            if (DebugManager.instance.displaying)
            {
                DebugManager.instance.ToggleHUD();
            }
            Time.timeScale = 0;
        }
        else //currently paused, unpause
        {
            gameState = GameState.PLAYING;
            AudioManager.instance.ResumeMusic();
            PauseMenu.instance.TurnOff(false);
            Time.timeScale = 1;
        }
    }
    internal void ResumeGameFromLoad()
    {
        gameState = GameState.PLAYING;
        switch (gameSession.stage)
        {
            case 1:
                SceneManager.LoadScene("Stage1");
                break;
            case 2:
                SceneManager.LoadScene("Stage2");
                break;
            default:
                Debug.LogError("Stage"+gameSession.stage+" does not exist");
                break;
        }
    }

    public void NextStage()
    {
        HUD.instance.FadeOutScreen();
        switch (gameSession.stage) {
            case 1:
                gameSession.stage = 2;
                SceneManager.LoadScene("Stage2");
                break;
            case 2:
                WellDoneMenu.instance.WellDone();
                break;
            default:
                Debug.LogError("Invalid stage!");
                break;
        }
        HUD.instance.FadeInScreen();
    }
}
