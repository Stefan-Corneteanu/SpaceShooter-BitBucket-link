using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyPattern : MonoBehaviour
{
    private int UID;
    public List<EnemyStep> steps = new List<EnemyStep>();

    public Enemy enemyPrefab = null;
    private Enemy spawnedEnemy = null;

    public bool stayOnLast = true;

    private int crtStateIdx = 0;
    private int prevStateIdx = -1;

    public bool startActive = false;

    public bool spawnOnEasy = true;
    public bool spawnOnNormal = true;
    public bool spawnOnHard = false;
    public bool spawnOnInsane = false;

    [HideInInspector]
    public Vector3 lastPos = Vector3.zero;
    [HideInInspector]
    public Vector3 crtPos = Vector3.zero;
    [HideInInspector]
    public Quaternion lastAngle = Quaternion.identity;

    public WaveTrigger owningWave = null;

#if UNITY_EDITOR
    [MenuItem("GameObject/SpaceShooter/EnemyPattern", false, 10)]
    static void CreateEnemyPatternObj(MenuCommand menuCmd)
    {
        Helpers helper = Resources.Load<Helpers>("Helper");
        if (helper)
        {
            GameObject GO = new GameObject("EnemyPattern"+helper.nextFreePatternID);
            EnemyPattern pattern = GO.AddComponent<EnemyPattern>();
            pattern.UID = helper.nextFreePatternID++;

            //Register creation w/ undo sys
            Undo.RegisterCreatedObjectUndo(GO,"Create "+GO.name);
            Selection.activeObject = GO;
        }
        else
        {
            Debug.LogError("Could not find helper!");
        }
    }
#endif
    private void Start()
    {
        if (startActive)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (spawnedEnemy)
        {
            return;
        }
        spawnedEnemy = Instantiate(enemyPrefab,transform.position, transform.rotation).GetComponent<Enemy>();
        spawnedEnemy.SetWave(owningWave);
        spawnedEnemy.SetPattern(this);
        lastPos = spawnedEnemy.transform.position;
        crtPos = lastPos;
    }

    public void CalculateTransform(Transform enemyTransform, float progressTimer)
    {
        Vector3 newPos = CalculatePosition(progressTimer);
        Quaternion newRot = CalculateRotation(progressTimer);

        enemyTransform.SetPositionAndRotation(newPos, newRot);

        if (crtStateIdx != prevStateIdx)
        {
            if (prevStateIdx >= 0)
            {
                //call deactivate states
                EnemyStep prevStep = steps[prevStateIdx];
                prevStep.FireDeactivateStates(spawnedEnemy);
            }

            if (crtStateIdx >=0)
            {
                //call activate states
                EnemyStep crtStep = steps[crtStateIdx];
                crtStep.FireActivateStates(spawnedEnemy);
            }
            prevStateIdx = crtStateIdx;
        }
    }

    public Vector3 CalculatePosition(float progressTimer)
    {
        crtStateIdx = WhichStep(progressTimer);
        if (crtStateIdx < 0)
        {
            if (spawnedEnemy)
            {
                return spawnedEnemy.transform.position;
            }
            else
            {
                return Vector2.zero;
            }
        }

        lastPos = crtPos;

        EnemyStep step = steps[crtStateIdx];

        float stepTime = progressTimer - StartTime(crtStateIdx);

        Vector3 startPos = EndPos(crtStateIdx - 1);

        crtPos = step.CalculatePosition(startPos, stepTime, lastPos, lastAngle);

        return crtPos;
    }

    public float TotalTime()
    {
        float time = 0f;
        foreach (EnemyStep step in steps)
        {
            time += step.TimeToComplete();
        }
        return time;
    }

    public Quaternion CalculateRotation(float progressTimer)
    {

        crtStateIdx = WhichStep(progressTimer);
        float startRot = 0;
        if (crtStateIdx > 0)
        {
            startRot = steps[crtStateIdx - 1].EndRot();
        }
        else if (crtStateIdx < 0)
        {
            return Quaternion.identity;
        }
        float stepTime = progressTimer - StartTime(crtStateIdx);
        lastAngle = steps[crtStateIdx].CalculateRotation(startRot, crtPos, lastPos, stepTime);
        return lastAngle;
    }

    int WhichStep(float progressTimer)
    {
        float timeToCheck = progressTimer;
        for (int i = 0; i < steps.Count; i++)
        {
            if (timeToCheck< steps[i].TimeToComplete())
            {
                return i;
            }
            timeToCheck -= steps[i].TimeToComplete();
        }
        if (stayOnLast)
            return steps.Count - 1;
        else
            return -1;
    }

    float StartTime(int step)
    {
        if (step <= 0)
        {
            return 0;
        }
        else
        {
            float result = 0;
            for (int i = 0; i < step; i++)
            {
                result+=steps[i].TimeToComplete();
            }
            return result;
        }
    }

    Vector3 EndPos(int step)
    {
        Vector3 result = transform.position;
        if (step >= 0)
        {
            for (int i = 0; i <= step; i++)
            {
                result = steps[i].EndPos(result);
            }
        }
        return result;
    }

    public EnemyStep AddStep(EnemyStep.MvmtType type)
    {
        EnemyStep step = new EnemyStep(type);
        steps.Add(step);
        return step;
    }

    public void OnValidate()
    {
        foreach (EnemyStep step in steps)
        {
            if (step.mvmtType == EnemyStep.MvmtType.SPLINE)
            {
                step.spline.CalculatePoints(step.mvmtSpeed);
            }
        }
    }

    internal bool ShouldSpawn()
    {
        Session.Difficulty diff;
        if (GameManager.instance && GameManager.instance.gameSession != null)
        {
            diff = GameManager.instance.gameSession.diff;
        }
        else
        {
            Debug.LogWarning("No game manager instance detected in EnemyPattern::ShouldSpawn. This should not happen in the final game." +
                " Defaulting to normal difficulty");
            diff = Session.Difficulty.NORMAL;
        }
        return spawnOnEasy && diff == Session.Difficulty.EASY
        || spawnOnNormal && diff == Session.Difficulty.NORMAL
        || spawnOnHard && diff == Session.Difficulty.HARD
        || spawnOnInsane && diff == Session.Difficulty.INSANE;
    }
}
