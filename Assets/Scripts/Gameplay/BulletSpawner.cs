using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public BulletManager.BulletType bulletType = BulletManager.BulletType.B1_S1;

    public BulletSeq seq;

    public GameObject muzzleFlash = null;

    public int rate = 1;
    public float speed = 0.5f;

    public float startAngle = 0;
    public float endAngle = 0;
    public int radialNumber = 1;
    public float dAngle = 0;

    public int timer = 0;

    public bool autoFireActive = false;
    private bool firing = false;
    private int frame = 0;

    public bool fireAtPlayer = false;
    public bool fireAtTarget = false;
    public GameObject target = null;
    public bool cleverShot = false;

    public bool isHoming = false;

    private byte playerIdx = 2; // (0 -> player1, 1 -> player2, 2 or anything else, enemy)

    public SFX shootSounds = null;

    private void Start()
    {
        Craft playerCraft = GetComponentInParent<Craft>();
        if (playerCraft)
        {
            playerIdx = playerCraft.playerIdx; // 0 or 1
        }
        else // enemy
        {
            playerIdx = 2;
        }
    }

    public void Shoot(int size)
    {
        if (size < 0)
        {
            return;
        }

        else
        {
            if (playerIdx !=0 && playerIdx != 1)
            {
                float x = transform.position.x;
                float y = transform.position.y;
                if (GameManager.instance
                && GameManager.instance.progressWindow)
                {
                    x -= GameManager.instance.progressWindow.transform.position.x;
                    y -= GameManager.instance.progressWindow.transform.position.y;
                }
                if (y < -3 || y >= 10 || x < -8.5 || x > 8.5)
                {
                    return;
                }
            }

            Vector2 primaryDirection = transform.up;

            if (!GameManager.instance.playerCrafts[0] && (!GameManager.instance.player2Exists || !GameManager.instance.playerCrafts[1]))
            {
                fireAtPlayer = false;
            }

            if (fireAtPlayer || fireAtTarget)
            {
                Vector3 targetPos = Vector3.zero;
                if (fireAtPlayer)
                {
                    targetPos = GameManager.instance.playerCrafts[0].transform.position;
                    if (GameManager.instance.playerCrafts[1]
                        && (Vector3.Distance(GameManager.instance.playerCrafts[1].transform.position, transform.position)
                        < Vector3.Distance(GameManager.instance.playerCrafts[0].transform.position, transform.position)))
                    {
                        targetPos = GameManager.instance.playerCrafts[1].transform.position;
                    }
                }
                else if (fireAtTarget && target)
                {
                    targetPos = target.transform.position;
                }

                primaryDirection = targetPos - transform.position;
                primaryDirection.Normalize();
            }

            if (timer == 0 || firing)
            {
                float angle = startAngle;
                for (int i = 0; i < radialNumber; i++)
                {
                    Quaternion rot = Quaternion.AngleAxis(angle,Vector3.forward);
                    Vector3 velocity = rot * primaryDirection * speed;
                    BulletManager.BulletType bullet = bulletType + size;
                    GameManager.instance.bulletManager.spawnBullet(bullet, transform.position.x, transform.position.y, velocity.x, velocity.y,
                        angle, dAngle, isHoming, playerIdx);
                    angle += (endAngle - startAngle) / (radialNumber - 1);
                }
                
                if (muzzleFlash)
                {
                    muzzleFlash.SetActive(true);
                }     

                if (shootSounds)
                {
                    shootSounds.Play();
                }
            }
        }
    }
    private void FixedUpdate()
    {
        timer++;
        if (timer >= rate)
        {
            timer = 0;
            if (muzzleFlash && !InputManager.instance.playerState[playerIdx].shoot)
            {
                muzzleFlash.SetActive(false);
            }

            if (autoFireActive)
            {
                firing = true;
                frame = 0;
            }
        }

        if (firing)
        {
            if (seq.ShouldFire(frame))
            {
                Shoot(1);
            }
            frame++;
            if (frame > seq.totalFrames)
            {
                firing = false;
            }
        }
    }

    public void ActivateAutoFire()
    {
        autoFireActive = true;
        frame = 0;
        firing = true;
    }

    public void DeactivateAutoFire()
    {
        autoFireActive = false;
    }

}

[Serializable]
public class BulletSeq
{
    public List<int> emitFrames = new List<int>();
    public int totalFrames;

    public bool ShouldFire (int crtframe)
    {
        foreach (int frame in emitFrames)
        {
            if (frame == crtframe)
            {
                return true;
            }
        }
        return false;
    }
}
