using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickupConfig", menuName = "SpaceShooter/PickupConfiguration")]
public class PickupConfig : ScriptableObject
{
    public Pickup.PickupType type;
    public int pow = 1;
    public int bombPow = 1;
    public int medalVal = 100;
    public float fallSpeed = 0;
    public int coinVal = 1;
    public int medalLvl = 1; 
    public int surplusVal = 100;
}
