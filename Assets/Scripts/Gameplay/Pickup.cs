using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        INVALID,
        BOMB,
        COIN,
        POWERUP,
        LASERBEAM,
        OPTION,
        MEDAL,
        SECRET,
        LIFE,
        NOPICKUPTYPES
    }

    public PickupConfig config;
    public Vector2 pos;
    public Vector2 vel;
    public SFX pickupSounds;
    private void OnEnable()
    {
        pos = transform.position;
        vel.x = UnityEngine.Random.Range(-0.04f, 0.04f);
        vel.y = UnityEngine.Random.Range(-0.04f, 0.04f);
    }
    private void FixedUpdate()
    {
        pos += vel;
        vel /= 1.15f; 
        pos.y -= config.fallSpeed;
        if (GameManager.instance && GameManager.instance.progressWindow)
        {
            float posY = pos.y - GameManager.instance.progressWindow.transform.position.y;
            if (posY < -5)
            {
                GameManager.instance.PickupFallOffScreen(this);
                Destroy(gameObject);
                return;
            }
        }
            transform.position = pos;
    }
    internal void ProcessPickup(int playerIdx, CraftData craftData)
    {
        if (pickupSounds)
        {
            pickupSounds.Play();
        }
        
        switch (config.type)
        {
            case PickupType.COIN:
                {
                    ScoreManager.instance.PickupCollected(playerIdx, config.coinVal);
                    break;
                }
            case PickupType.POWERUP:
                {
                    GameManager.instance.playerCrafts[playerIdx].Powerup((byte)config.pow, config.surplusVal);
                    break;
                }
            case PickupType.LIFE:
                {
                    GameManager.instance.playerCrafts[playerIdx].OneUp(config.surplusVal);
                    break;
                }
            case PickupType.SECRET:
                {
                    ScoreManager.instance.PickupCollected(playerIdx, config.coinVal);
                    break;
                }
            case PickupType.MEDAL:
                {
                    ScoreManager.instance.MedalCollected(playerIdx, config.medalVal);
                    GameManager.instance.playerCrafts[playerIdx].AddMedal(config.medalLvl, config.medalVal);
                    break;
                }
            case PickupType.LASERBEAM:
                {
                    GameManager.instance.playerCrafts[playerIdx].IncreaseBeamStr(config.surplusVal);
                    break;
                }
            case PickupType.OPTION:
                {
                    GameManager.instance.playerCrafts[playerIdx].AddOption(config.surplusVal);
                    break;
                }
            case PickupType.BOMB:
                {
                    GameManager.instance.playerCrafts[playerIdx].AddBomb(config.bombPow, config.surplusVal);
                    break;
                }
            default:
                {
                    Debug.LogError("Unknown type of pickup: " + config.type);
                    break;
                }
        }
        Destroy(gameObject);
    }
}
