using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    public EnemyRule[] rules;

    private EnemyPattern pattern;

    private EnemySection[] sections;

    public bool isBoss = false;

    private int timer = 0;
    public int timeout = 3600;
    private bool timedout = false;

    Animator anim = null;
    public String timeoutParamName = null;

    private WaveTrigger owningWave = null;

    private void Start()
    {
        sections = gameObject.GetComponentsInChildren<EnemySection>();
        anim = gameObject.GetComponentInChildren<Animator>();
        timer = timeout;
    }

    public void SetPattern (EnemyPattern pattern)
    {
        this.pattern = pattern;
    }

    private void FixedUpdate()
    {

        //timeout
        if (isBoss)
        {
            if (timer<0 && !timedout)
            {
                timedout = true;
                if (anim)
                {
                    anim.SetTrigger(timeoutParamName);
                }
                sections[0].EnableState("FlyOut");
            }
            else
            {
                timer--;
            }
        }

        data.progressTimer++;

        if (pattern)
        {
            pattern.CalculateTransform(transform, data.progressTimer);
        }

        //off-screen check
        float y = transform.position.y;
        if (GameManager.instance && GameManager.instance.progressWindow)
        {
            y -= GameManager.instance.progressWindow.transform.position.y;
        }

        if (y < -5)
        {
            OutOfBounds();
        }

        //update state timers
        foreach (EnemySection section in sections)
        {
            section.UpdateStateTimers();
        }

    }

    public void EnableState(string name)
    {
        foreach (EnemySection section in sections)
        {
            section.EnableState(name);
        }
    }

    public void DisableState(string name)
    {
        foreach (EnemySection section in sections)
        {
            section.DisableState(name);
        }
    }

    public void OutOfBounds()
    {
        Destroy(gameObject);
    }

    public void TimeoutDestruct()
    {
        Destroy(gameObject);

        if (isBoss)
        {
            GameManager.instance.NextStage();
        }
    }

    internal void PartsDestroyed()
    {
        //Go through all rules, check for parts matching ruleset
        foreach(EnemyRule rule in rules)
        {
            if (!rule.isTriggered)
            {
                int countDestroyedParts = 0;
                foreach (EnemyPart part in rule.partsToCheck)
                {
                    if (part.isDestroyed)
                    {
                        countDestroyedParts++;
                    }
                }
                if (countDestroyedParts >= rule.noPartsRequired)
                {
                    rule.isTriggered = true;
                    rule.ruleEvents.Invoke();
                }
            }
        }
    }

    public void Destroyed(int triggeredFromRuleIdx)
    {
        EnemyRule triggeredRule = rules[triggeredFromRuleIdx];
        int playerIdx = triggeredRule.partsToCheck[0].destroyedByPlayer; //TODO : check that using first idx is safe
        if (owningWave)
        {
            owningWave.EnemyDestroyed(transform.position, playerIdx);
        }

        Destroy(gameObject);

        if (isBoss)
        {
            GameManager.instance.NextStage();
        }
    }

    public void SetWave(WaveTrigger wave)
    {
        owningWave = wave;
    }
}

[Serializable]
public struct EnemyData
{
    public float progressTimer;
    public float posX;
    public float posY;

    public int patternUID;
}
