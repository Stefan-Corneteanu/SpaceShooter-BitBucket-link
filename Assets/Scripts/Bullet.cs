using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int idx; //bullet pool idx
    public byte playerIdx; //player that shot this bullet
}

[Serializable]
public struct BulletData
{
    public float posX;
    public float posY;
    public float dX;
    public float dY;
    public float angle;
    public float dAngle;
    public int type;
    public bool isActive;
    public bool isHoming;

    public BulletData(float posX, float posY, float dX, float dY, float angle, float dAngle, int type, bool isActive, bool isHoming)
    {
        this.posX = posX;
        this.posY = posY;
        this.dX = dX;
        this.dY = dY;
        this.angle = angle;
        this.dAngle = dAngle;
        this.type = type;
        this.isActive = isActive;
        this.isHoming = isHoming;
    }
}