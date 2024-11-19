using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    public ProgressData data;

    public float levelSize;

    //public GameObject midgroundTilemap = null;

    //public float midgroundrate = 0.75f;

    public AnimationCurve speedCurve = new AnimationCurve();

    //private float initPosY;

    private Craft player1Craft;
    private Craft player2Craft;

    // Start is called before the first frame update
    void Start()
    {
        //initPosY = transform.position.y;
        data.posX = transform.position.x;
        data.posY = transform.position.y;

        if (GameManager.instance)
        {
            GameManager.instance.progressWindow = this;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player1Craft || player2Craft)
        {
            if (data.progress < levelSize)
            {
                float ratio = data.progress / levelSize;
                float mvmt = speedCurve.Evaluate(ratio);
                data.progress++;
                UpdateProgressWindow(mvmt);
            }
            UpdateWindowX(GameManager.instance.gameSession.craftDatas[0].posX);
        }
        else
        {
            if (GameManager.instance)
            {
                player1Craft = GameManager.instance.playerCrafts[0];
                if (GameManager.instance.player2Exists)
                {
                    player2Craft = GameManager.instance.playerCrafts[1];
                }
                if (!GameManager.instance.progressWindow)
                {
                    GameManager.instance.progressWindow = this;
                }
            }
        }
    }

    void UpdateProgressWindow(float mvmt)
    {
        data.posY += mvmt;
        transform.position = new Vector3(data.posX, data.posY, 0);
        //midgroundTilemap.transform.position = new Vector3(0, -initPosY + data.posY * midgroundrate, 0);
    }

    void UpdateWindowX(float craftX)
    {
        if (Math.Abs(craftX) > 6.5f && Math.Abs(transform.position.x) < 2.5f)
        {
            data.posX += craftX / 1000;
            transform.position = new Vector3(data.posX, data.posY, 0);
        }
        else if (transform.position.x >=0.01f )
        {
            data.posX -= (6.5f-craftX) / 1000;
            transform.position = new Vector3(data.posX, data.posY, 0);
        }
        else if (transform.position.x <= -0.01f)
        {
            data.posX -= (-6.5f - craftX) / 1000;
            transform.position = new Vector3(data.posX, data.posY, 0);
        }
    }
}

//! Removed the separate speed with which the midground would move, reintroduce it if requested

[Serializable]
public class ProgressData
{
    public int progress;
    public float posX;
    public float posY;
}
