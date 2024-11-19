using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;
    public InputState[] playerState = new InputState[2];
    public InputState[] prevPlayerState = new InputState[2];
    public ButtonMapping[] playerButtons = new ButtonMapping[2];
    public AxisMapping[] playerAxis = new AxisMapping[2];
    public KeyButtonMapping[] playerKeyButtons = new KeyButtonMapping[2];
    public KeyAxisMapping[] playerKeyAxis = new KeyAxisMapping[2];

    public int[] playerController = new int[2];
    public bool[] playerUsesKeyboard = new bool[2];

    public const float deadZone = 0.01f;

    private System.Array allKeyCodes = System.Enum.GetValues(typeof(KeyCode));

    private string[,] playerButtonNames = {{"J1_B1", "J1_B2", "J1_B3", "J1_B4", "J1_B5", "J1_B6", "J1_B7", "J1_B8"},
                                           {"J2_B1", "J2_B2", "J2_B3", "J2_B4", "J2_B5", "J2_B6", "J2_B7", "J2_B8"},
                                           {"J3_B1", "J3_B2", "J3_B3", "J3_B4", "J3_B5", "J3_B6", "J3_B7", "J3_B8"},
                                           {"J4_B1", "J4_B2", "J4_B3", "J4_B4", "J4_B5", "J4_B6", "J4_B7", "J4_B8"},
                                           {"J5_B1", "J5_B2", "J5_B3", "J5_B4", "J5_B5", "J5_B6", "J5_B7", "J5_B8"},
                                           {"J6_B1", "J6_B2", "J6_B3", "J6_B4", "J6_B5", "J6_B6", "J6_B7", "J6_B8"}};

    private string[,] playerAxisNames = {{"J1_Horizontal", "J1_Vertical" },
                                         {"J2_Horizontal", "J2_Vertical" },
                                         {"J3_Horizontal", "J3_Vertical" },
                                         {"J4_Horizontal", "J4_Vertical" },
                                         {"J5_Horizontal", "J5_Vertical" },
                                         {"J6_Horizontal", "J6_Vertical" }};

    public string[] oldJoysticks = null;

    public static string[] actionNames = { "Shoot", "Bomb", "Options", "Auto", "Beam", "Menu", "Extra2", "Extra3" };
    public static string[] axisNames = { "Left", "Right", "Up", "Down" };

    public enum Action
    {
        SHOOT,
        BOMB,
        OPTIONS,
        AUTO,
        BEAM,
        MENU,
        EXTRA2,
        EXTRA3
    }

    public enum Axis
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("InputManager created");

            //initialization
            playerController[0] = -1;
            playerController[1] = -1;

            playerAxis[0] = new AxisMapping();
            playerAxis[1] = new AxisMapping();

            playerButtons[0] = new ButtonMapping();
            playerButtons[1] = new ButtonMapping();

            playerKeyAxis[0] = new KeyAxisMapping(0);
            playerKeyAxis[1] = new KeyAxisMapping(1);

            playerKeyButtons[0] = new KeyButtonMapping(0);
            playerKeyButtons[1] = new KeyButtonMapping(1);

            playerState[0] = new InputState();
            playerState[1] = new InputState();

            prevPlayerState[0] = new InputState();
            prevPlayerState[1] = new InputState();

            playerUsesKeyboard[0] = false;
            playerUsesKeyboard[1] = false;

            oldJoysticks = Input.GetJoystickNames();

            StartCoroutine(CheckControllers());
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra input manager");
            Destroy(gameObject);
        }
    }

    IEnumerator CheckControllers()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            string[] crtJoysticks = Input.GetJoystickNames();

            for (int i = 0; i < crtJoysticks.Length; i++)
            {
                if (i < oldJoysticks.Length)
                {
                    if (crtJoysticks[i] != oldJoysticks[i])
                    {
                        if (string.IsNullOrEmpty(crtJoysticks[i])) //controller disconnect
                        {
                            Debug.Log("Controller " + i + " has been disconnected");
                            if (PlayerIsUsingController(i))
                            {
                                ControllerMenu.instance.player = i;
                                ControllerMenu.instance.disconnectedText.text = "Player"+(i+1)+" Controller disconnected";
                                ControllerMenu.instance.TurnOn(null);
                                //Game manager.instance.pauseGameplay
                            }
                        }
                        else //new controller
                        {
                            Debug.Log("Controller " + i + " is connected using: " + crtJoysticks[i]);
                        }
                    }
                }
                else
                {
                    Debug.Log("New controller connected");
                }
            }
        }
    }

    private bool PlayerIsUsingController(int idx)
    {
        return playerController[0] == idx || (GameManager.instance.player2Exists && playerController[1] == idx );
    }

    internal void BindPlayerButton(int player, int actionID, byte btn)
    {
        switch (actionID)
        {
            case 0: playerButtons[player].shoot = btn; break;
            case 1: playerButtons[player].bomb = btn; break;
            case 2: playerButtons[player].options = btn; break;
            case 3: playerButtons[player].auto = btn; break;
            case 4: playerButtons[player].beam = btn; break;
            case 5: playerButtons[player].menu = btn; break;
            case 6: playerButtons[player].extra2 = btn; break;
            case 7: playerButtons[player].extra3 = btn; break;
        }
    }

    internal void BindPlayerKey(int player, int actionID, KeyCode key)
    {
        switch (actionID)
        {
            case 0: playerKeyButtons[player].shoot = key; break;
            case 1: playerKeyButtons[player].bomb = key; break;
            case 2: playerKeyButtons[player].options = key; break;
            case 3: playerKeyButtons[player].auto = key; break;
            case 4: playerKeyButtons[player].beam = key; break;
            case 5: playerKeyButtons[player].menu = key; break;
            case 6: playerKeyButtons[player].extra2 = key; break;
            case 7: playerKeyButtons[player].extra3 = key; break;
        }
    }
    internal void BindPlayerAxisKey(int player, int actionID, KeyCode key)
    {
        switch (actionID)
        {
            case 0: playerKeyAxis[player].up = key; break;
            case 1: playerKeyAxis[player].down = key; break;
            case 2: playerKeyAxis[player].left = key; break;
            case 3: playerKeyAxis[player].right = key; break;
        }
    }

    void UpdatePlayerStates(int playerIdx)
    {
        prevPlayerState[playerIdx].CopyStateFrom(playerState[playerIdx]);

        playerState[playerIdx].left = false;
        playerState[playerIdx].right = false;
        playerState[playerIdx].up = false;
        playerState[playerIdx].down = false;

        playerState[playerIdx].shoot = false;
        playerState[playerIdx].bomb = false;
        playerState[playerIdx].options = false;
        playerState[playerIdx].auto = false;
        playerState[playerIdx].beam = false;

        playerState[playerIdx].menu = false;
        playerState[playerIdx].extra2 = false;
        playerState[playerIdx].extra3 = false;

        if (Input.GetKey(playerKeyAxis[playerIdx].left))
        {
            playerState[playerIdx].left = true;
        }

        if (Input.GetKey(playerKeyAxis[playerIdx].right))
        {
            playerState[playerIdx].right = true;
        }

        if (Input.GetKey(playerKeyAxis[playerIdx].up))
        {
            playerState[playerIdx].up = true;
        }

        if (Input.GetKey(playerKeyAxis[playerIdx].down))
        {
            playerState[playerIdx].down = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].shoot))
        {
            playerState[playerIdx].shoot = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].bomb))
        {
            playerState[playerIdx].bomb = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].options))
        {
            playerState[playerIdx].options = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].auto))
        {
            playerState[playerIdx].auto = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].beam))
        {
            playerState[playerIdx].beam = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].menu))
        {
            playerState[playerIdx].menu = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].extra2))
        {
            playerState[playerIdx].extra2 = true;
        }

        if (Input.GetKey(playerKeyButtons[playerIdx].extra3))
        {
            playerState[playerIdx].extra3 = true;
        }

        if (playerController[playerIdx] < 0)
        {
            UpdateMovement(playerIdx);
            return;
        }

        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIdx], playerAxis[playerIdx].horizontal]) < -deadZone) //going to the left
        {
            playerState[playerIdx].left = true;
        }

        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIdx], playerAxis[playerIdx].horizontal]) > deadZone) //going to the right
        {
            playerState[playerIdx].right = true;
        }

        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIdx], playerAxis[playerIdx].vertical]) > deadZone) //going upwards
        {
            playerState[playerIdx].up = true;
        }

        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIdx], playerAxis[playerIdx].vertical]) < -deadZone) //going downwards
        {
            playerState[playerIdx].down = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].shoot]))
        {
            playerState[playerIdx].shoot = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].bomb]))
        {
            playerState[playerIdx].bomb = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].options]))
        {
            playerState[playerIdx].options = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].auto]))
        {
            playerState[playerIdx].auto = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].beam]))
        {
            playerState[playerIdx].beam = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].menu]))
        {
            playerState[playerIdx].menu = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].extra2]))
        {
            playerState[playerIdx].extra2 = true;
        }

        if (Input.GetButton(playerButtonNames[playerController[playerIdx], playerButtons[playerIdx].extra3]))
        {
            playerState[playerIdx].extra3 = true;
        }

        UpdateMovement(playerIdx);
    }

    public bool StateActivated(int playerIdx, Action actionID)
    {
        switch (actionID)
        {
            case Action.SHOOT: return playerState[playerIdx].shoot == true && prevPlayerState[playerIdx].shoot == false;
            case Action.BOMB: return playerState[playerIdx].bomb == true && prevPlayerState[playerIdx].bomb == false;
            case Action.OPTIONS: return playerState[playerIdx].options == true && prevPlayerState[playerIdx].options == false;
            case Action.AUTO: return playerState[playerIdx].auto == true && prevPlayerState[playerIdx].auto == false;
            case Action.BEAM: return playerState[playerIdx].beam == true && prevPlayerState[playerIdx].beam == false;
            case Action.MENU: return playerState[playerIdx].menu == true && prevPlayerState[playerIdx].menu == false;
            case Action.EXTRA2: return playerState[playerIdx].extra2 == true && prevPlayerState[playerIdx].extra2 == false;
            case Action.EXTRA3: return playerState[playerIdx].extra3 == true && prevPlayerState[playerIdx].extra3 == false;
            default: throw new Exception("Invalid Action Id");
        }
    }

    public bool StateDectivated(int playerIdx, Action actionID)
    {
        switch (actionID)
        {
            case Action.SHOOT: return playerState[playerIdx].shoot == false && prevPlayerState[playerIdx].shoot == true;
            case Action.BOMB: return playerState[playerIdx].bomb == false && prevPlayerState[playerIdx].bomb == true;
            case Action.OPTIONS: return playerState[playerIdx].options == false && prevPlayerState[playerIdx].options == true;
            case Action.AUTO: return playerState[playerIdx].auto == false && prevPlayerState[playerIdx].auto == true;
            case Action.BEAM: return playerState[playerIdx].beam == false && prevPlayerState[playerIdx].beam == true;
            case Action.MENU: return playerState[playerIdx].menu == false && prevPlayerState[playerIdx].menu == true;
            case Action.EXTRA2: return playerState[playerIdx].extra2 == false && prevPlayerState[playerIdx].extra2 == true;
            case Action.EXTRA3: return playerState[playerIdx].extra3 == false && prevPlayerState[playerIdx].extra3 == true;
            default: throw new Exception("Invalid Action Id");
        }
    }

    public bool StateToogled(int playerIdx, Action actionID)
    {
        switch (actionID)
        {
            case Action.SHOOT: return playerState[playerIdx].shoot != prevPlayerState[playerIdx].shoot;
            case Action.BOMB: return playerState[playerIdx].bomb != prevPlayerState[playerIdx].bomb;
            case Action.OPTIONS: return playerState[playerIdx].options != prevPlayerState[playerIdx].options;
            case Action.AUTO: return playerState[playerIdx].auto != prevPlayerState[playerIdx].auto;
            case Action.BEAM: return playerState[playerIdx].beam != prevPlayerState[playerIdx].beam;
            case Action.MENU: return playerState[playerIdx].menu != prevPlayerState[playerIdx].menu;
            case Action.EXTRA2: return playerState[playerIdx].extra2 != prevPlayerState[playerIdx].extra2;
            case Action.EXTRA3: return playerState[playerIdx].extra3 != prevPlayerState[playerIdx].extra3;
            default: throw new Exception("Invalid Action Id");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerStates(0);
        UpdatePlayerStates(1);
    }

    public int DetectControllerButtonPress() //return controller idx
    {

        for (int joystick = 0; joystick < 6; joystick++)
        {
            for (int btn = 0; btn < 8; btn++)
            {
                if (Input.GetButton(playerButtonNames[joystick, btn]))
                {
                    return joystick;
                }
            }
        }

        return -1;
    }

    public int DetectButtonPress() //return button idx
    {

        for (int joystick = 0; joystick < 6; joystick++)
        {
            for (int btn = 0; btn < 8; btn++)
            {
                if (Input.GetButton(playerButtonNames[joystick, btn]))
                {
                    return btn;
                }
            }
        }

        return -1;
    }

    public int DetectKeyPress()
    {
        foreach (KeyCode key in allKeyCodes)
        {
            if (Input.GetKey(key))
            {
                return (int)key;
            }
        }
        return -1;
    }

    public bool CheckForPlayerInput(int playerIdx)
    {
        int controller = DetectControllerButtonPress();
        if ( controller > -1)
        {
            playerController[playerIdx] = controller;
            playerUsesKeyboard[playerIdx] = false;
            Debug.Log("Player " + playerIdx + " uses joystick " + controller);
            return true;
        }
        if (DetectKeyPress() > -1)
        {
            playerController[playerIdx] = -1;
            playerUsesKeyboard[playerIdx] = true;
            Debug.Log("Player " + playerIdx + " uses keyboard");
            return true;
        }
        return false;
    }

    internal string GetKeyName(int playerIdx, int actionID)
    {
        KeyCode key = KeyCode.None;
        switch (actionID)
        {
            case 0: key = playerKeyButtons[playerIdx].shoot; break;
            case 1: key = playerKeyButtons[playerIdx].bomb; break;
            case 2: key = playerKeyButtons[playerIdx].options; break;
            case 3: key = playerKeyButtons[playerIdx].auto; break;
            case 4: key = playerKeyButtons[playerIdx].beam; break;
            case 5: key = playerKeyButtons[playerIdx].menu; break;
            case 6: key = playerKeyButtons[playerIdx].extra2; break;
            case 7: key = playerKeyButtons[playerIdx].extra3; break;
        }
        return key.ToString();
    }

    internal string GetKeyAxisName(int playerIdx, int actionID)
    {
        KeyCode key = KeyCode.None;
        switch (actionID)
        {
            case 0: key = playerKeyAxis[playerIdx].up; break;
            case 1: key = playerKeyAxis[playerIdx].down; break;
            case 2: key = playerKeyAxis[playerIdx].left; break;
            case 3: key = playerKeyAxis[playerIdx].right; break;
        }
        return key.ToString();
    }

    internal string GetButtonName(int playerIdx, int actionID)
    {
        string btnName = "";
        switch (actionID)
        {
            case 0: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].shoot]; break;
            case 1: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].bomb]; break;
            case 2: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].options]; break;
            case 3: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].auto]; break;
            case 4: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].beam]; break;
            case 5: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].menu]; break;
            case 6: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].extra2]; break;
            case 7: btnName = playerButtonNames[playerIdx, playerButtons[playerIdx].extra3]; break;
        }
        char btnNo = btnName[4]; //WARNING: all playerButtonNames should respect format Jx_By and return y
        return "Button "+btnNo;
    }

    internal void UpdateMovement(int playerIdx)
    {

        playerState[playerIdx].mvmt.x = 0;
        playerState[playerIdx].mvmt.y = 0;

        if (playerState[playerIdx].right)
        {
            playerState[playerIdx].mvmt.x += 1;
        }
        
        if (playerState[playerIdx].left)
        {
            playerState[playerIdx].mvmt.x -= 1;
        }

        if (playerState[playerIdx].up)
        {
            playerState[playerIdx].mvmt.y += 1;
        }
        
        if (playerState[playerIdx].down)
        {
            playerState[playerIdx].mvmt.y -= 1;
        }

        playerState[playerIdx].mvmt.Normalize();
    }

    public void SaveKeyBindings()
    {
        playerKeyButtons[0].Save();
        playerKeyButtons[1].Save();
        playerKeyAxis[0].Save();
        playerKeyAxis[1].Save();
    }
}

public class InputState
{
    public Vector2 mvmt;
    public bool left, right, up, down;
    public bool shoot, bomb, options, auto, beam, menu, extra2, extra3;

    public void CopyStateFrom(InputState other)
    {
        this.mvmt = other.mvmt;

        this.left = other.left;
        this.right = other.right;
        this.up = other.up;
        this.down = other.down;

        this.shoot = other.shoot;
        this.bomb = other.bomb;
        this.options = other.options;
        this.auto = other.auto;
        this.beam = other.beam;
        this.menu = other.menu;
        this.extra2 = other.extra2;
        this.extra3 = other.extra3;
    }
}

public class ButtonMapping
{
    public byte shoot = 0;
    public byte bomb = 1;
    public byte options = 2;
    public byte auto = 3;
    public byte beam = 4;
    public byte menu = 5;
    public byte extra2 = 6;
    public byte extra3 = 7;
}

public class AxisMapping
{
    public byte horizontal = 0;
    public byte vertical = 1;
}

public class KeyButtonMapping
{
    private byte playerIdx;

    public KeyCode shoot;
    public KeyCode bomb;
    public KeyCode options;
    public KeyCode auto;
    public KeyCode beam;
    public KeyCode menu;
    public KeyCode extra2;
    public KeyCode extra3;

    public KeyButtonMapping(byte playerIdx)
    {
        this.playerIdx = playerIdx;
        if (playerIdx == 0) //P1
        {
            shoot = PlayerPrefs.HasKey("P" + playerIdx + "shoot") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "shoot") : KeyCode.B;
            bomb = PlayerPrefs.HasKey("P" + playerIdx + "bomb") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "bomb") : KeyCode.N;
            options = PlayerPrefs.HasKey("P" + playerIdx + "options") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "options") : KeyCode.C;
            auto = PlayerPrefs.HasKey("P" + playerIdx + "auto") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "auto") : KeyCode.LeftAlt;
            beam = PlayerPrefs.HasKey("P" + playerIdx + "beam") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "beam") : KeyCode.LeftControl;
            menu = PlayerPrefs.HasKey("P" + playerIdx + "menu") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "menu") : KeyCode.Escape;
            extra2 = PlayerPrefs.HasKey("P" + playerIdx + "extra2") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "extra2") : KeyCode.E;
            extra3 = PlayerPrefs.HasKey("P" + playerIdx + "extra3") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "extra3") : KeyCode.F;
        }
        else //P2
        {
            shoot = PlayerPrefs.HasKey("P" + playerIdx + "shoot") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "shoot") : KeyCode.Keypad0;
            bomb = PlayerPrefs.HasKey("P" + playerIdx + "bomb") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "bomb") : KeyCode.KeypadPeriod;
            options = PlayerPrefs.HasKey("P" + playerIdx + "options") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "options") : KeyCode.KeypadEnter;
            auto = PlayerPrefs.HasKey("P" + playerIdx + "auto") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "auto") : KeyCode.LeftAlt;
            beam = PlayerPrefs.HasKey("P" + playerIdx + "beam") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "beam") : KeyCode.KeypadPlus;
            menu = PlayerPrefs.HasKey("P" + playerIdx + "menu") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "menu") : KeyCode.Escape;
            extra2 = PlayerPrefs.HasKey("P" + playerIdx + "extra2") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "extra2") : KeyCode.Keypad8;
            extra3 = PlayerPrefs.HasKey("P" + playerIdx + "extra3") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "extra3") : KeyCode.Keypad9;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("P" + playerIdx + "shoot", (int)shoot);
        PlayerPrefs.SetInt("P" + playerIdx + "bomb", (int)bomb);
        PlayerPrefs.SetInt("P" + playerIdx + "options", (int)options);
        PlayerPrefs.SetInt("P" + playerIdx + "auto", (int)auto);
        PlayerPrefs.SetInt("P" + playerIdx + "beam", (int)beam);
        PlayerPrefs.SetInt("P" + playerIdx + "menu", (int)menu);
        PlayerPrefs.SetInt("P" + playerIdx + "extra2", (int)extra2);
        PlayerPrefs.SetInt("P" + playerIdx + "extra3", (int)extra3);
    }
}

public class KeyAxisMapping
{
    private byte playerIdx;

    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public KeyAxisMapping(byte playerIdx)
    {
        this.playerIdx = playerIdx;
        if (playerIdx == 0) //P1
        {
            up = PlayerPrefs.HasKey("P" + playerIdx + "up") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "up") : KeyCode.W;
            down = PlayerPrefs.HasKey("P" + playerIdx + "down") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "down") : KeyCode.S;
            left = PlayerPrefs.HasKey("P" + playerIdx + "left") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "left") : KeyCode.A;
            right = PlayerPrefs.HasKey("P" + playerIdx + "right") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "right") : KeyCode.D;
        }
        else //P2
        {
            up = PlayerPrefs.HasKey("P" + playerIdx + "up") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "up") : KeyCode.UpArrow;
            down = PlayerPrefs.HasKey("P" + playerIdx + "down") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "down") : KeyCode.DownArrow;
            left = PlayerPrefs.HasKey("P" + playerIdx + "left") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "left") : KeyCode.LeftArrow;
            right = PlayerPrefs.HasKey("P" + playerIdx + "right") ? (KeyCode)PlayerPrefs.GetInt("P" + playerIdx + "right") : KeyCode.RightArrow;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("P" + playerIdx + "up", (int)up);
        PlayerPrefs.SetInt("P" + playerIdx + "down", (int)down);
        PlayerPrefs.SetInt("P" + playerIdx + "left", (int)left);
        PlayerPrefs.SetInt("P" + playerIdx + "right", (int)right);
    }
}

