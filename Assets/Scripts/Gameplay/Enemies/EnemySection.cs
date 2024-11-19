using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySection : MonoBehaviour
{
    public List<EnemyState> states = new List<EnemyState>();

    public void EnableState(string name)
    {
        foreach (EnemyState state in states)
        {
            if (state.name.ToLower() == name.ToLower())
            {
                state.Enable();
            }
        }
    }

    public void DisableState(string name)
    {
        foreach (EnemyState state in states)
        {
            if (state.name.ToLower() == name.ToLower())
            {
                state.Disable();
            }
        }
    }

    public void UpdateStateTimers()
    {
        foreach (EnemyState state in states)
        {
            if (state.isActive)
            {
                state.IncrementTime();
            }
        }
    }



    public void TimeoutMsg()
    {
        Enemy enemy = transform.parent.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TimeoutDestruct();
        }
    }
}
