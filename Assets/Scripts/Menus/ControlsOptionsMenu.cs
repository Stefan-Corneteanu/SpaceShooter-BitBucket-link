using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsOptionsMenu : Menu
{
    public static ControlsOptionsMenu instance = null;

    public Button[] p1_buttons = new Button[8];
    public Button[] p1_keys = new Button[12];

    public Button[] p2_buttons = new Button[8];
    public Button[] p2_keys = new Button[12];

    public GameObject bindingPanel = null;
    public Text bindText = null;
    public EventSystem eventSystem = null;

    private bool bindingButton = false;
    private bool bindingKey = false;
    private bool bindingAxis = false;

    public int actionBinding = -1;
    public int playerBinding = -1;

    private bool waiting = false;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra controls options menu");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        UpdateButtons();
    }

    public void OnBackButton()
    {
        InputManager.instance.SaveKeyBindings();
        TurnOff(true);
    }

    void UpdateButtons()
    {
        //joystick buttons
        for (int btn = 0; btn < 8; btn++)
        {
            p1_buttons[btn].GetComponentInChildren<Text>().text = InputManager.instance.GetButtonName(0,btn);
            p2_buttons[btn].GetComponentInChildren<Text>().text = InputManager.instance.GetButtonName(1, btn);
        }

        //equivalent keys
        for (int key = 0; key < 8; key++)
        {
            p1_keys[key].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyName(0, key);
            p2_keys[key].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyName(1, key);
        }

        //keyboard axis

        for (int key = 0; key < 4; key++)
        {
            p1_keys[key+8].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyAxisName(0, key);
            p2_keys[key+8].GetComponentInChildren<Text>().text = InputManager.instance.GetKeyAxisName(1, key);
        }
    }

    public void BindP1Button(int actionId)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for Player1 " + InputManager.actionNames[actionId];
        bindingPanel.SetActive(true);
        bindingButton = true;
        bindingKey = false;
        bindingAxis = false;
        actionBinding = actionId;
        playerBinding = 0;
        waiting = true;
    }
    public void BindP1Key(int actionId)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player1 " + InputManager.actionNames[actionId];
        bindingPanel.SetActive(true);
        bindingButton = false;
        bindingKey = true;
        bindingAxis = false;
        actionBinding = actionId;
        playerBinding = 0;
        waiting = true;
    }

    public void BindP1AxisKey(int actionId)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player1 " + InputManager.axisNames[actionId];
        bindingPanel.SetActive(true);
        bindingButton = false;
        bindingKey = true;
        bindingAxis = true;
        actionBinding = actionId;
        playerBinding = 0;
        waiting = true;
    }

    public void BindP2Button(int actionId)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for Player2 " + InputManager.actionNames[actionId];
        bindingPanel.SetActive(true);
        bindingButton = true;
        bindingKey = false;
        bindingAxis = false;
        actionBinding = actionId;
        playerBinding = 1;
        waiting = true;
    }
    public void BindP2Key(int actionId)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player2 " + InputManager.actionNames[actionId];
        bindingPanel.SetActive(true);
        bindingButton = false;
        bindingKey = true;
        bindingAxis = false;
        actionBinding = actionId;
        playerBinding = 1;
        waiting = true;
    }

    public void BindP2AxisKey(int actionId)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player2 " + InputManager.axisNames[actionId];
        bindingPanel.SetActive(true);
        bindingButton = false;
        bindingKey = true;
        bindingAxis = true;
        actionBinding = actionId;
        playerBinding = 1;
        waiting = true;
    }

    private void Update()
    {
        if (bindingKey || bindingButton)
        {
            if (waiting)
            {
                if (Input.anyKey || InputManager.instance.DetectButtonPress() > -1) //any key on keyboard or any button on controller still pressed
                {
                    return;
                }
                waiting = false;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    bindingPanel.SetActive(false);
                    bindingKey = false;
                    bindingButton = false;
                    bindingAxis = false;
                    eventSystem.gameObject.SetActive(true);
                }
                if (bindingKey)
                {
                    foreach (KeyCode key in KeyCode.GetValues(typeof(KeyCode)))
                    {
                        if (!key.ToString().Contains("Joystick"))
                        {
                            if (Input.GetKeyDown(key)) //key press
                            {
                                if (bindingAxis)
                                {
                                    InputManager.instance.BindPlayerAxisKey(playerBinding, actionBinding, key);
                                }
                                else
                                {
                                    InputManager.instance.BindPlayerKey(playerBinding, actionBinding, key);
                                }
                                bindingPanel.SetActive(false);
                                bindingKey = false;
                                bindingButton = false;
                                bindingAxis = false;
                                eventSystem.gameObject.SetActive(true);
                                UpdateButtons();
                            }
                        }
                    }
                }
                else if (bindingButton)
                {
                    int btn = InputManager.instance.DetectButtonPress();
                    if (btn > -1) //button press
                    {
                        InputManager.instance.BindPlayerButton(playerBinding, actionBinding, (byte)btn);
                        bindingPanel.SetActive(false);
                        bindingKey = false;
                        bindingButton = false;
                        bindingAxis = false;
                        eventSystem.gameObject.SetActive(true);
                        UpdateButtons();
                    }
                }
            }
        }
    }
}
