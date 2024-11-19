using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Session
{
    public enum Difficulty
    {
        EASY,
        NORMAL,
        HARD,
        INSANE
    };
    
    public Difficulty diff = Difficulty.NORMAL;
    public int stage = 1;
    public bool isPractice = false;
    public bool isArenaPractice = false;
    public bool isStagePractice = false;

    public CraftData[] craftDatas = new CraftData[2];

    public byte P1SelectedShip = 0;
    public byte P2SelectedShip = 0;

    //cheats

    public bool hasInfLives = false;
    public bool hasInfContinues = false;
    public bool hasInfBombs = false;
    public bool hasInvincibility = false;
    public bool isHalfSpeed = false;
    public bool isDoubleSpeed = false;
    public Session()
    {
        craftDatas[0] = new CraftData();
        craftDatas[1] = new CraftData();
    }

    internal void Save(BinaryWriter bw)
    {
        craftDatas[0].Save(bw);
        if (GameManager.instance.player2Exists)
        {
            craftDatas[1].Save(bw);
        }
        bw.Write((byte)diff);
        bw.Write(stage);
    }

    internal void Load(BinaryReader br)
    {
        craftDatas[0].Load(br);
        if (GameManager.instance.player2Exists)
        {
            craftDatas[1].Load(br);
        }
        diff = (Difficulty)br.ReadByte();
        stage = br.ReadInt32();
    }
}
