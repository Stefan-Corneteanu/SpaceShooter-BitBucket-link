using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public enum ColliderType
    {
        INVALID,
        CIRCLE,
        BOX,
        POLYGON
    }
    public int health = 10;
    public float radius = 0.16f;
    public float height = 0.1f;
    public float width = 0.1f;
    private new Collider2D collider;
    private ColliderType colliderType;
    private Vector3 halfExtent;

    private int layerMask = 0;

    public bool damagedByBullets = true;
    public bool damagedByBeams = true;
    public bool damagedByBombs = true;

    public bool remainDestroyed = false;
    private bool destroyed = false;
    public int damageHealth = 5; // at what health is damaged sprite displayed

    public bool spawnCyclicPickups = false;
    public Pickup[] spawnSpecificPickups;

    public int hitScore = 10;
    public int destroyScore = 1000;
    public SFX destroyedSounds = null;

    private bool isFlashing = false;
    private float flashTimer = 0f;

    private SpriteRenderer sr = null;

    public bool hasLargeExplosion = false;
    public bool hasSmallExplosion = false;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") &
            ~LayerMask.GetMask("GroundEnemy") &
            ~LayerMask.GetMask("EnemyBullets");

        sr = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        if (collider)
        {
            if (collider.GetType() == typeof(BoxCollider2D))
            {
                colliderType = ColliderType.BOX;
                halfExtent = new Vector3(width / 2, height / 2, 1);
            }
            else if (collider.GetType() == typeof(CircleCollider2D))
            {
                colliderType = ColliderType.CIRCLE;
            }
            else if (collider.GetType() == typeof(PolygonCollider2D))
            {
                colliderType = ColliderType.POLYGON;
            }
            else
            {
                colliderType = ColliderType.INVALID;
                Debug.LogError("Collider type not known in shootable");
            }
        }
        else
        {
            Debug.LogError("Shootable has no collider set");
        }
    }

    private void FixedUpdate()
    {
        if (destroyed)
        {
            return;
        }

        if (isFlashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                sr.material.SetColor("_OverBright",Color.black);
                isFlashing = false;
            }
        }

        Debug.Assert(colliderType != ColliderType.INVALID);
        int maxColliders = 10;
        Collider2D[] hits = new Collider2D[maxColliders];
        int noHits = 0;
        if (colliderType == ColliderType.CIRCLE)
        {
            noHits = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hits, layerMask);
        }
        else if (colliderType == ColliderType.BOX)
        {
            noHits = Physics2D.OverlapBoxNonAlloc(transform.position, halfExtent, transform.eulerAngles.z, hits, layerMask);
        }
        else if (colliderType == ColliderType.POLYGON)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(layerMask);
            contactFilter.useLayerMask = true;
            noHits = Physics2D.OverlapCollider(collider, contactFilter, hits);
        }
        
        if (noHits > 0)
        {
            for (int i = 0; i < noHits; i++)
            {
                if (damagedByBullets)
                {
                    Bullet bullet = hits[i].GetComponent<Bullet>();
                    if (bullet)
                    {
                        TakeDamage(GameManager.instance.gameSession.craftDatas[bullet.playerIdx].shotPower, bullet.playerIdx);
                        GameManager.instance.bulletManager.DeactivateBullet(bullet.idx);
                        FlashAndSpark(bullet.transform.position);
                    }
                }
                
                if (damagedByBombs)
                {
                    Bomb bomb = hits[i].GetComponent<Bomb>();
                    if (bomb && damagedByBombs)
                    {
                        TakeDamage(bomb.power, (byte)bomb.playerIdx);
                        FlashAndSpark(transform.position);
                    }
                }
            }
        }
    }
    public void TakeDamage(int dmg, byte shooterPlayerIdx)
    {
        if (destroyed)
        {
            return;
        }

        ScoreManager.instance.ShootableHit(shooterPlayerIdx, hitScore);

        health -= dmg;

        EnemyPart part = GetComponent<EnemyPart>();
        if (part)
        {
            if (health <= damageHealth)
            {
                part.Damaged(true);
            }
            else
            {
                part.Damaged(false);
            }
        }
        
        if (health <= 0) //destroyed
        {
            destroyed = true;
            if (part)
            {
                part.Destroyed(shooterPlayerIdx);
            }
            
            if (destroyedSounds)
            {
                destroyedSounds.Play();
            }

            if (shooterPlayerIdx == 0 || shooterPlayerIdx == 1)
            {
                ScoreManager.instance.ShootableDestroyed(shooterPlayerIdx, destroyScore);
                GameManager.instance.playerDatas[shooterPlayerIdx].chainLevel++;
                ScoreManager.instance.UpdateChainMultiplier(shooterPlayerIdx);
                GameManager.instance.playerDatas[shooterPlayerIdx].chainTimer = PlayerData.MAX_CHAIN_TIMER;
            }
            Vector2 pos = transform.position;
            if (spawnCyclicPickups)
            {
                Pickup spawnPrefab = GameManager.instance.GetNextDrop();
                GameManager.instance.SpawnPickup(spawnPrefab,pos);
            }
            foreach (Pickup pickupPrefab in spawnSpecificPickups)
            {
                GameManager.instance.SpawnPickup(pickupPrefab, pos);
            }

            if (hasSmallExplosion)
            {
                FXSys.instance.SpawnSmallExplosion(transform.position);
            }

            if (hasLargeExplosion)
            {
                FXSys.instance.SpawnLargeExplosion(transform.position);
            }

            if (remainDestroyed)
            {
                destroyed = true;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void FlashAndSpark(Vector3 pos)
    {
        FXSys.instance.SpawnSparks(pos);
        if (isFlashing)
        {
            return;
        }

        isFlashing = true;
        flashTimer = 0.01f;
        sr.material.SetColor("_OverBright", Color.white);
    }
}
