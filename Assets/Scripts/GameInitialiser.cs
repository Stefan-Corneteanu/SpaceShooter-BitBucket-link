using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitialiser : MonoBehaviour
{

    public enum GameMode
    {
        INVALID,MENUS,GAMEPLAY
    }

    public GameMode gameMode;

    public GameObject gameManagerPrefab = null;

    private bool isInitialised = false;

    private Scene displayScene;

    public AudioManager.Tracks playMusicTrack = AudioManager.Tracks.NONE;

    public int stageNumber = 0;
    void Awake()
    {
        if (GameManager.instance == null)
        {
            if (gameManagerPrefab)
            {
                Instantiate(gameManagerPrefab);
                displayScene = SceneManager.GetSceneByName("Display");
            }
            else
            {
                Debug.LogError("Error: Game Manager Prefab not set");
            }
        }
    }

    void Update()
    {
        if (!isInitialised)
        {
            if (gameMode == GameMode.INVALID)
            {
                return;
            }
            if (!displayScene.isLoaded)
            {
                SceneManager.LoadScene("Display",LoadSceneMode.Additive);
            }
            switch (gameMode)
            {
                case GameMode.MENUS:
                    MenuManager.instance.SwitchToMainMenuMenus();
                    GameManager.instance.gameState = GameManager.GameState.INMENUS;
                    break;

                case GameMode.GAMEPLAY:
                    MenuManager.instance.SwitchToGameplayMenus();
                    GameManager.instance.gameState = GameManager.GameState.PLAYING;
                    GameManager.instance.gameSession.stage = stageNumber;
                    break;
            }
            if (playMusicTrack != AudioManager.Tracks.NONE)
            {
                AudioManager.instance.PlayMusic(playMusicTrack, true, 1);
            }

            if (gameMode == GameMode.GAMEPLAY)
            {
                SaveManager.instance.SaveGame(0); //autosaves at slot 0 at the beginning of each stage
                GameManager.instance.SpawnPlayers();
            }
            isInitialised = true;
        }
    }
        
    
}
