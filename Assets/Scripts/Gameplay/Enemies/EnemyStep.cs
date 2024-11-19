using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStep
{
    public enum MvmtType
    {
        INVALID,
        NONE,
        DIRECTION,
        SPLINE,
        ATTARGET,
        HOMING,
        FOLLOW,
        CIRCLE,
        NOMVMTTYPES
    }

    public enum RotType
    {
        INVALID,
        NONE,
        SETANGLE,
        LOOKAHEAD,
        SPINNING,
        FACEPLAYER,
        NOROTTYPES
    }

    [SerializeField]
    public MvmtType mvmtType;

    [SerializeField]
    public RotType rotType = RotType.LOOKAHEAD;

    [SerializeField]
    public float endAngle = 0f;

    [SerializeField]
    [Range(0.01f,1f)]
    public float angleSpeed = 0.01f;

    [SerializeField]
    public float noSpins = 1;

    [SerializeField]
    public Vector2 direction;

    [SerializeField]
    public Spline spline;

    [Range(0.01f,1f)]
    [SerializeField]
    public float mvmtSpeed = 0.04f;

    [SerializeField]
    public float framesToWait = 60;

    public List<String> activateStates = new List<string>();
    public List<String> deactivateStates = new List<string>();
    private MvmtType type;

    public EnemyStep(MvmtType type)
    {
        this.type = type;
        direction = Vector2.zero;
        if (type == MvmtType.SPLINE)
        {
            this.spline = new Spline();
        }
    }

    public float TimeToComplete()
    {
        float timeToTravel;
        switch (mvmtType)
        {
            case MvmtType.DIRECTION:
                timeToTravel = direction.magnitude / mvmtSpeed;
                return timeToTravel;
            case MvmtType.NONE:
                return framesToWait;
            case MvmtType.SPLINE:
                timeToTravel = spline.Length() / mvmtSpeed;
                return timeToTravel;
            case MvmtType.HOMING:
                return framesToWait;
            default:
                Debug.LogError("TimeToComplete unprocessed movement type, returning 1");
                return 1;
        }
    }

    public Vector2 EndPos(Vector3 startPos)
    {
        Vector2 result = startPos;
        switch (mvmtType)
        {
            case MvmtType.DIRECTION:
                result += direction;
                break;
            case MvmtType.NONE:
                break;
            case MvmtType.SPLINE:
                result += spline.LastPoint() - spline.FirstPoint();
                break;
            case MvmtType.HOMING:
                if (GameManager.instance && GameManager.instance.playerCrafts[0])
                {
                    result = GameManager.instance.playerCrafts[0].transform.position;
                    if (GameManager.instance.playerCrafts[1] 
                        && (Vector3.Distance(GameManager.instance.playerCrafts[1].transform.position , startPos)
                        < Vector3.Distance(GameManager.instance.playerCrafts[0].transform.position, startPos)))
                    {
                        result = GameManager.instance.playerCrafts[1].transform.position;
                    }
                }
                break;
            default:
                Debug.LogError("TimeToComplete unprocessed movement type, returning initial position");
                break;
        }
        return result;
    }

    public Vector3 CalculatePosition(Vector3 startPos, float stepTime, Vector2 oldPos, Quaternion oldAngle)
    {
        Vector2 result = startPos;
        float normalizedTime = stepTime / TimeToComplete();
        normalizedTime = Math.Max(normalizedTime, 0);
        switch (mvmtType)
        {
            case MvmtType.DIRECTION:
                float timeToTravel = direction.magnitude / mvmtSpeed;
                float ratio = 0;
                if (timeToTravel != 0)
                {
                    ratio = stepTime / timeToTravel;
                }
                result += direction*ratio;
                break;
            case MvmtType.NONE:
                break;
            case MvmtType.SPLINE:
                result += spline.GetPosition(normalizedTime);
                break;
            case MvmtType.HOMING:
                if (GameManager.instance && GameManager.instance.playerCrafts[0])
                {
                    Craft homedOnCraft = GameManager.instance.playerCrafts[0];
                    if (GameManager.instance.playerCrafts[1]
                        && (Vector3.Distance(GameManager.instance.playerCrafts[1].transform.position, startPos)
                        < Vector3.Distance(GameManager.instance.playerCrafts[0].transform.position, startPos)))
                    {
                        homedOnCraft = GameManager.instance.playerCrafts[1];
                    }
                        Vector2 dir = (homedOnCraft.transform.position - startPos).normalized;
                    Vector2 mvmt = (dir * mvmtSpeed);
                    result = oldPos + mvmt;
                }
                break;
            default:
                Debug.LogError("CalculatePosition unprocessed movement type, returning initial position");
                break;
        }
        return result;
    }

    public void FireActivateStates(Enemy enemy)
    {
        foreach (string state in activateStates)
        {
            enemy.EnableState(state);
        }
    }

    internal float EndRot() //TODO this is unfinished
    {
        return endAngle;
    }

    public void FireDeactivateStates(Enemy enemy)
    {
        foreach (string state in deactivateStates)
        { 
            enemy.DisableState(state);
        }
    }

    internal Quaternion CalculateRotation(float startRot, Vector3 crtPos, Vector3 lastPos, float time)
    {
        float normalizedTime = time / TimeToComplete();
        if (normalizedTime < 0)
        {
            normalizedTime = 0;
        }

        switch (rotType)
        {
            case RotType.SETANGLE:
                {
                    Quaternion result = Quaternion.Euler(0, 0, endAngle);
                    return result;
                }
            case RotType.SPINNING:
                {
                    float startAngle = endAngle - (noSpins * 360);
                    float angle = Mathf.Lerp(startAngle, endAngle, normalizedTime);
                    Quaternion result = Quaternion.Euler(0, 0, angle); 
                    return result;
                }
            case RotType.FACEPLAYER:
                {
                    float angle = 0;
                    Transform target = null;
                    if (GameManager.instance && GameManager.instance.playerCrafts[0])
                    {
                        target = GameManager.instance.playerCrafts[0].transform;
                        if (GameManager.instance.playerCrafts[1]
                        && (Vector3.Distance(GameManager.instance.playerCrafts[1].transform.position, crtPos)
                        < Vector3.Distance(GameManager.instance.playerCrafts[0].transform.position, crtPos)))
                        {
                            target = GameManager.instance.playerCrafts[1].transform;
                        }
                    }
                    if (target != null)
                    {
                        Vector2 crtDir = (crtPos - lastPos).normalized;
                        Vector2 tarDir = (target.position - crtPos).normalized;
                        Vector2 newDir = Vector2.Lerp(crtDir, tarDir, angleSpeed);
                        angle = Vector2.SignedAngle(Vector2.down, newDir);
                    }
                    return Quaternion.Euler(0, 0, angle);
                }
            case RotType.LOOKAHEAD:
                {
                    Vector2 dir = (crtPos - lastPos).normalized;
                    float angle = Vector2.SignedAngle(Vector2.down, dir);
                    return Quaternion.Euler(0, 0, angle);
                }
            default:
                {
                    return Quaternion.Euler(Vector3.zero);
                }
        }
    }
}
