using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int score = 0;
    public int stageScore = 0;
    public byte lives = 3;

    public int chainLevel = 0;
    public byte chainTimer = 0;
    public static byte MAX_CHAIN_TIMER = 200;

    internal void Save(BinaryWriter bw)
    {
        bw.Write(score);
        bw.Write(stageScore);
        bw.Write(lives);
    }

    internal void Load(BinaryReader br)
    {
        score = br.ReadInt32();
        stageScore = br.ReadInt32();
        lives = br.ReadByte();
    }

    internal void ResetData()
    {
        score = 0;
        stageScore = 0;
        lives = 3;
        chainLevel = 0;
        chainTimer = 0;
    }
}
