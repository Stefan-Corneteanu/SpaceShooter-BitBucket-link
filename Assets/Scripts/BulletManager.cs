using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class BulletManager : MonoBehaviour
{
    public Bullet[] bulletPrefabs;

    public enum BulletType
    {
        B1_S1,
        B1_S2,
        B1_S3,
        B1_S4,
        B1_S5,
        B2_S1,
        B2_S2,
        B2_S3,
        B2_S4,
        B3_S1,
        B3_S2,
        B3_S3,
        B3_S4,
        B4_S1,
        B4_S2,
        B4_S3,
        B4_S4,
        B5_S1,
        B5_S2,
        B5_S3,
        B5_S4,
        B6_S1,
        B6_S2,
        B6_S3,
        B6_S4,
        B7_S1,
        B7_S2,
        B7_S3,
        B7_S4,
        B8_S1,
        B8_S2,
        B8_S3,
        B8_S4,
        B9_S1,
        B9_S2,
        B9_S3,
        B9_S4,
        B10_S1,
        B10_S2,
        B10_S3,
        B10_S4,
        B11_S1,
        B11_S2,
        B11_S3,
        B12_S1,
        B12_S2,
        B12_S3,
        B13_S1,
        B13_S2,
        B13_S3,
        B13_S4,
        B13_S5,
        B13_S6,
        B13_S7,
        B13_S8,
        B13_S9,
        B13_S10,
        B13_S11,
        B13_S12,
        B13_S13,
        B13_S14,
        B13_S15,
        B13_S16,
        B14_S1,
        B14_S2,
        B14_S3,
        B14_S4,
        B14_S5,
        B14_S6,
        B14_S7,
        B14_S8,
        B15_S1,
        B15_S2,
        B15_S3,
        B15_S4,
        B15_S5,
        B15_S6,
        B15_S7,
        B15_S8,
        B16_S1,
        B16_S2,
        B16_S3,
        B16_S4,
        MAX_TYPES
    }

    private const int MAX_BULLET_PER_TYPE = 100;
    private const int MAX_BULLET_COUNT = MAX_BULLET_PER_TYPE * (int)BulletType.MAX_TYPES;

    private Bullet[] bulletPool = new Bullet[MAX_BULLET_COUNT];
    private NativeArray<BulletData> bulletsData;
    private TransformAccessArray bulletsTransforms;

    ProcessBulletJob processBulletJob;

    // Start is called before the first frame update
    void Start()
    {
        bulletsData = new NativeArray<BulletData>(MAX_BULLET_COUNT,Allocator.Persistent);
        bulletsTransforms = new TransformAccessArray(MAX_BULLET_COUNT);
        processBulletJob = new ProcessBulletJob(bulletsData);

        int idx = 0;
        for (int bulletType = (int)BulletType.B1_S1; bulletType < (int)BulletType.MAX_TYPES; bulletType++)
        {
            for (int i = 0; i < MAX_BULLET_PER_TYPE; i++)
            {
                Bullet bullet = Instantiate(bulletPrefabs[bulletType].GetComponent<Bullet>());
                bullet.idx = idx;
                bullet.gameObject.SetActive(false);
                bullet.transform.SetParent(transform);
                bulletsTransforms.Add(bullet.transform);
                bulletPool[idx++] = bullet;
            }
        }
    }

    private void OnDestroy()
    {
        bulletsData.Dispose();
        bulletsTransforms.Dispose();
    }

    private int findNextFreeBulletIdx(BulletType bulletType)
    {
        int startIdx = (int)bulletType * MAX_BULLET_PER_TYPE;
        for (int i = 0; i < MAX_BULLET_PER_TYPE; i++)
        {
            if (!bulletsData[startIdx + i].isActive)
            {
                return startIdx + i;
            }
        }
        return -1; //no bullet of that type found
    }

    public Bullet spawnBullet(BulletType bulletType, float x, float y, float dX, float dY, float angle, float dAngle, bool isHoming, byte playerIdx)
    {
        int idx = findNextFreeBulletIdx(bulletType);
        if (idx != -1)
        {
            Bullet bullet = bulletPool[idx];
            bullet.playerIdx = playerIdx;
            bulletsData[idx] = new BulletData(x,y,dX,dY,angle,dAngle,(int)bulletType, true, isHoming);
            bullet.gameObject.transform.position = new Vector3(x, y, 0);
            bullet.gameObject.SetActive(true);

            SpriteRenderer sr = bullet.gameObject.GetComponent<SpriteRenderer>();
            if (sr)
            {
                switch (playerIdx)
                {
                    case 0:
                        sr.color = new Color(0.54f, 0.89f, 0.92f, 1f);
                        break;
                    case 1:
                        sr.color = new Color(1f, 0.84f, 0f, 1f);
                        break;
                    case 2:
                    default:
                        sr.color = new Color(1f, 0f, 1f, 1f);
                        break;
                }
            }

            return bullet;
        }
        else
        {
            return null;
        }
    }

    internal void DeactivateBullet(int idx)
    {
        bulletPool[idx].gameObject.SetActive(false);
        float x = bulletsData[idx].posX;
        float y = bulletsData[idx].posY;
        float dX = bulletsData[idx].dX;
        float dY = bulletsData[idx].dY;
        float angle = bulletsData[idx].angle;
        float dAngle = bulletsData[idx].dAngle;
        int type = bulletsData[idx].type;
        bool isHoming = bulletsData[idx].isHoming;
        bulletsData[idx] = new BulletData(x, y, dX, dY, angle, dAngle, type, false, isHoming);
    }


    private void FixedUpdate()
    {
        if (GameManager.instance && GameManager.instance.playerCrafts[0])
        {
            processBulletJob.player1Pos = GameManager.instance.playerCrafts[0].transform.position;
        }
        else
        {
            processBulletJob.player1Pos = new Vector3(-9999,-9999);
        }

        if (GameManager.instance && GameManager.instance.progressWindow)
        {
            processBulletJob.progressY = GameManager.instance.progressWindow.transform.position.y;
        }
        else
        {
            processBulletJob.progressY = 0;
        }

        ProcessBullets();
        for(int i = 0; i < MAX_BULLET_COUNT; i++)
        {
            if (!bulletsData[i].isActive)
            {
                bulletPool[i].gameObject.SetActive(false);
            }
        }
    }

    void ProcessBullets()
    {
        JobHandle jobHandle = processBulletJob.Schedule(bulletsTransforms);
        jobHandle.Complete();
    }

    public struct ProcessBulletJob : IJobParallelForTransform
    {
        public NativeArray<BulletData> bulletsData;
        public Vector3 player1Pos;
        public Vector3 player2Pos;
        public float progressY;
        public ProcessBulletJob(NativeArray<BulletData> bulletsData)
        {
            this.bulletsData = bulletsData;
            player1Pos = player2Pos = new Vector3(-9999, -9999);
            progressY = 0;
        }
        public void Execute(int index, TransformAccess transform)
        {
            bool isActive = bulletsData[index].isActive;
            if (!isActive)
            {
                return;
            }
            else
            {
                float x = bulletsData[index].posX;
                float y = bulletsData[index].posY;
                float dX = bulletsData[index].dX;
                float dY = bulletsData[index].dY;
                float angle = bulletsData[index].angle;
                float dAngle = bulletsData[index].dAngle;
                int type = bulletsData[index].type;
                bool isHoming = bulletsData[index].isHoming;

                //Homing
                if ((player1Pos.x < -1000 
                || player1Pos.x > 1000 
                || player1Pos.y - progressY < -1000 
                || player1Pos.y - progressY > 1000)
                && (player2Pos.x < -1000
                || player2Pos.x > 1000
                || player2Pos.y - progressY < -1000
                || player2Pos.y - progressY > 1000))
                {
                    isActive = false;
                }
                else if (isHoming)
                {
                    Vector2 velocity = new Vector2(dX, dY);
                    float speed = velocity.magnitude;
                    Vector2 toPlayer1 = new Vector2(player1Pos.x - x, player1Pos.y - y);
                    Vector2 toPlayer2 = new Vector2(player2Pos.x - x, player2Pos.y - y);
                    Vector2 toPlayer = toPlayer1.magnitude < toPlayer2.magnitude ? toPlayer1 : toPlayer2; //homing on closest of players
                    Vector2 newVelocity = Vector2.Lerp(velocity.normalized, toPlayer.normalized, 0.05f).normalized;
                    newVelocity *= speed;
                    dX = newVelocity.x;
                    dY = newVelocity.y;
                }

                //Mvmt
                x += dX;
                y += dY;

                //Out of screen bounds check
                if (x < -9 
                || x > 9 
                || y - progressY < -5 
                || y - progressY > 5)
                {
                    isActive = false;
                }

                bulletsData[index] = new BulletData(x, y, dX, dY, angle, dAngle, type, isActive, isHoming);

                if (isActive)
                {
                    Vector3 newPos = new Vector3(x, y, 0);
                    transform.position = newPos;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(dX,dY,0));
                }
            }
        }
    }
}
