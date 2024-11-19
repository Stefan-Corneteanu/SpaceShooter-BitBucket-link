using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPart : MonoBehaviour
{
    public bool isDestroyed = false;
    public bool isDamaged = false;
    private bool usingDamagedSprite = false;

    public Sprite damagedSprite = null;
    public Sprite destroyedSprite = null;

    public UnityEvent triggerOnDestroyed;

    public int destroyedByPlayer = 2;
    public void Destroyed(int playerIdx)
    {
        if (isDestroyed)
            return;
        destroyedByPlayer = playerIdx;
        triggerOnDestroyed.Invoke();

        if (destroyedSprite)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                sr.sprite = destroyedSprite;
            }
        }
        isDestroyed = true;
        Enemy enemy = transform.root.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.PartsDestroyed();
        }
    }

    public void Damaged(bool switchToDamagedSprite)
    {
        if (isDestroyed)
            return;

        if (switchToDamagedSprite && !usingDamagedSprite)
        {
            if (damagedSprite)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr)
                {
                    sr.sprite = damagedSprite;
                    usingDamagedSprite = true;
                }
            }
            isDamaged = switchToDamagedSprite;
        }
    }
}
