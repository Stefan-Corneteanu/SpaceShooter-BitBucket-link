using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject root = null;
    public Menu previousMenu = null;
    private GameObject previousItem = null;

    public virtual void TurnOn(Menu previous) 
    { 
        if (!root)
        {
            Debug.LogError("Root object not set");
        }
        else
        {
            if (previous != null)
            {
                previousMenu = previous;
            }

            root.SetActive(true);

            if (previousItem)
            {
                EventSystem.current.SetSelectedGameObject(previousItem);
            }
        }
    }
    public virtual void TurnOff(bool returnToPrev) 
    {
        if (!root)
        {
            Debug.LogError("Root object not set");
        }
        else
        {
            if (EventSystem.current)
            {
                previousItem = EventSystem.current.currentSelectedGameObject;
            }
            root.SetActive(false);

            if (previousMenu && returnToPrev)
            {
                previousMenu.TurnOn(null);
            }
        }
    }


}
