using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;
    public int crtMultiplier = 1;

    public int[,] scores = new int[8, 4];
    public string[,] names = new string[8, 4];
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("ScoreManager created");

            LoadScores();
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra score manager");
            Destroy(gameObject);
        }
    }

    public void AddScore(int score, Session.Difficulty diff, string name)
    {
        for (int i = 0; i < 8; i++)
        {
            if (score > scores[i, (int)diff]) //insert here
            {
                ShuffleScoresDown(i, diff);
                scores[i, (int)diff] = score;
                names[i, (int)diff] = name;
                return;
            }
        }
    }

    internal int GetTopScore(Session.Difficulty diff)
    {
        return scores[0, (int)diff];
    }

    private void ShuffleScoresDown(int idx, Session.Difficulty diff)
    {
        for (int i = 7; i > idx; i--)
        {
            scores[i, (int)diff] = scores[i - 1, (int)diff];
            names[i, (int)diff] = names[i - 1, (int)diff];
        }
    }

    public bool IsTopScore(int score, Session.Difficulty diff)
    {
        return score >= scores[0, (int)diff];
    }

    public bool IsHiScore(int score, Session.Difficulty diff)
    {
        return score > scores[7, (int)diff];
    }

    internal bool IsScoreHigherThan2ndLowestHighScore(int score, Session.Difficulty diff)
    {
        return score > scores[6, (int)diff];
    }

    public void SaveScores()
    {
        string savePath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("savePath=" + savePath);
        try
        {
            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bw.Write(names[i, j]);
                    bw.Write(scores[i, j]);
                }
            }
            bw.Close();
            fs.Close();
        }
        catch (Exception ex) 
        {
            Debug.LogWarning(ex.Message + "Failed to create filestream or binary writer for saving hiscores");
        }
    }
    public void LoadScores()
    {
        string loadPath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("loadPath=" + loadPath);
        try
        {
            FileStream fs = new FileStream(loadPath, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    names[i, j] = br.ReadString();
                    scores[i, j] = br.ReadInt32();
                }
            }
            br.Close();
            fs.Close();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message + "Failed to create filestream or binary reader for loading hiscores");
        }
    }

    public void DeleteScoresFile()
    {
        string filePath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("filePath=" + filePath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    public void ShootableHit(int playerIdx, int score)
    {
        GameManager.instance.playerCrafts[playerIdx].IncreaseScore(score * crtMultiplier);
    }

    public void ShootableDestroyed(int playerIdx, int score)
    {
        GameManager.instance.playerCrafts[playerIdx].IncreaseScore(score * crtMultiplier);
    }

    public void BossDestroyed(int playerIdx, int score)
    {
        GameManager.instance.playerCrafts[playerIdx].IncreaseScore(score * crtMultiplier);
    }

    public void PickupCollected(int playerIdx, int score)
    {
        GameManager.instance.playerCrafts[playerIdx].IncreaseScore(score * crtMultiplier);
    }

    public void BulletDestroyed(int playerIdx, int score)
    {
        GameManager.instance.playerCrafts[playerIdx].IncreaseScore(score * crtMultiplier);
    }

    public void MedalCollected(int playerIdx, int score)
    {
        GameManager.instance.playerCrafts[playerIdx].IncreaseScore(score * crtMultiplier);
    }

    public void UpdateChainMultiplier(int playerIdx)
    {
        int chain = GameManager.instance.playerDatas[playerIdx].chainLevel;
        crtMultiplier = (int)Math.Pow(chain + 1, 1.5);
    }
}
