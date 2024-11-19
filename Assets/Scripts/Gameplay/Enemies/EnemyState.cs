using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyState
{
    public String name;
    public bool isActive = false;

    [Space(10)]
    [Header("--Start Events--")]
    [Space(10)]
    public UnityEvent eventOnStart = null;
    [Space(10)]
    [Header("--Stop Events--")]
    [Space(10)]
    public UnityEvent eventOnEnd = null;
    [Space(10)]
    [Header("--Timer Events--")]
    [Space(10)]
    public UnityEvent eventOnTimer = null;

    public bool usesTimer = false;
    public float timer = 0;
    private float crtTime = 0;

    public void Enable()
    {
        isActive = true;
        crtTime = 0;
        eventOnStart.Invoke();
    }

    public void Disable()
    {
        isActive = false;
        eventOnEnd.Invoke();
    }

    public void IncrementTime()
    {
        if (usesTimer)
        {
            crtTime++;
            if (crtTime >= timer)
            {
                eventOnTimer.Invoke();
                crtTime = 0;
            }
        }
    }
}
