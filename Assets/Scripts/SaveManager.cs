using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;

    const int SAVE_VERSION = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("DebugManager created");
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra save manager");
            Destroy(gameObject);
        }
    }

    public void SaveGame(int saveSlot)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(SAVE_VERSION);
            bw.Write(GameManager.instance.player2Exists);
            GameManager.instance.gameSession.Save(bw);
            GameManager.instance.playerDatas[0].Save(bw);
            if (GameManager.instance.player2Exists)
            {
                GameManager.instance.playerDatas[1].Save(bw);
            }

            string savePath = Application.persistentDataPath + "/slot" + saveSlot + ".dat";
            Debug.Log("Save path = " + savePath);

            FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
            ms.WriteTo(fs);
            fs.Close();
            bw.Close();
            ms.Close();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message + "Failed to create filestream or binary writer for saving");
        }
    }

    public void LoadGame(int saveSlot)
    {
        try
        {
            string loadPath = Application.persistentDataPath + "/slot" + saveSlot + ".dat";
            MemoryStream ms = new MemoryStream();
            FileStream fs = new FileStream(loadPath, FileMode.Open);
            BinaryReader br = new BinaryReader(ms);
            fs.CopyTo(ms);
            ms.Position = 0;
            int version = br.ReadInt32();
            if (version == SAVE_VERSION)
            {
                GameManager.instance.player2Exists = br.ReadBoolean();
                GameManager.instance.gameSession.Load(br);
                GameManager.instance.playerDatas[0].Load(br);
                if (GameManager.instance.player2Exists)
                {
                    GameManager.instance.playerDatas[1].Load(br);
                }

                GameManager.instance.ResumeGameFromLoad();
            }
            else
            {
                Debug.LogError("Incorrect save file version");
            }
            br.Close();
            fs.Close();
            ms.Close();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message + "Failed to create filestream or binary reader for loading");
        }
    }

    public void CopySaveToSlot(int saveSlot)
    {
        Debug.Assert(saveSlot > 0, "Slot 0 reserved for autosave");
        try
        {
            string loadPath = Application.persistentDataPath + "/slot0.dat";
            string destPath = Application.persistentDataPath + "/slot" + saveSlot + ".dat";
            File.Copy(loadPath, destPath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message + "Failed to copy save to slot" + saveSlot);
        }
    }

    public bool LoadExists(int saveSlot)
    {
        string loadPath = Application.persistentDataPath + "/slot" + saveSlot + ".dat";
        return File.Exists(loadPath);
    }
}
