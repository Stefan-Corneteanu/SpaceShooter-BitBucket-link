using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftConfig", menuName = "SpaceShooter/CraftConfiguration")]
public class CraftConfig : ScriptableObject
{
    public static int MAX_SPEED = 10;
    public static int MAX_SHOT_POW = 10;
    public static int MAX_BEAM_POW = 10;
    public static int MAX_BOMB_POW = 10;
    public static int MAX_OPTION_POW = 10;

    public float speed;
    public byte bulletStr;
    public byte beamStr;
    public byte bombStr;
    public byte optionStr;

    public Sprite craftSprite;

    public ShotConfig[] shotLevel = new ShotConfig[MAX_SHOT_POW];
}

[Serializable]
public class ShotConfig
{
    public int[] spawnerSizes = new int[5];
}