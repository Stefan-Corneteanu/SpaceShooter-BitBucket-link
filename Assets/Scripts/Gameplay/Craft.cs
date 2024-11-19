using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Craft : MonoBehaviour
{
    public CraftConfig config;
    Vector3 newPos = new Vector3();
    public byte playerIdx;
    public GameObject flame1;
    public GameObject flame2;
    public GameObject aftFlame1;
    public GameObject aftFlame2;

    Animator anim;
    int leftBoolId, rightBoolId;

    internal bool isAlive = true;
    bool isInvulnerable = true;

    int invulnerableTimer = 120;
    const int INVULNERABLELENGTH = 120;
    public static int MAX_BEAM_CHARGE = 150;

    SpriteRenderer spriteRenderer = null;

    private int layerMask = 0;
    private int pickupLayer = 0;

    public BulletSpawner[] bulletSpawners = new BulletSpawner[5];

    public Option[] options = new Option[4];

    public GameObject[] optionMarkersL1 = new GameObject[4];
    public GameObject[] optionMarkersL2 = new GameObject[4];
    public GameObject[] optionMarkersL3 = new GameObject[4];
    public GameObject[] optionMarkersL4 = new GameObject[4];

    public Beam beam = null;

    public GameObject bombPrefab = null;

    public static byte MAX_LIVES = 5;

    public static byte MAX_SMALL_BOMBS = 8;
    public static byte MAX_BIG_BOMBS = 5;

    public SFX explodingNoise = null;
    public SFX bombSounds = null;
    private void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Assert(anim);

        leftBoolId = Animator.StringToHash("Left");
        rightBoolId = Animator.StringToHash("Right");

        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(spriteRenderer);

        layerMask = ~LayerMask.GetMask("PlayerBullets") 
                  & ~LayerMask.GetMask("PlayerBombs") 
                  & ~LayerMask.GetMask("Player")
                  & ~LayerMask.GetMask("GroundEnemy");
        pickupLayer = LayerMask.NameToLayer("Pickup");
    }

    internal void SetPlayerIdx(byte playerIdx)
    {
        this.playerIdx = playerIdx;
        if (!spriteRenderer) //does not get it in start!?
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        switch (playerIdx)
        {
            case 0:
                spriteRenderer.color = new Color(0.54f, 0.89f, 0.92f, 1f);
                break;
            case 1:
                spriteRenderer.color = new Color(1f, 0.84f, 0f, 1f);
                break;
            case 2:
            default:
                spriteRenderer.color = new Color(1f, 0f, 1f, 1f);
                break;
        }
    }

    public void SetInvulnerable()
    {
        isInvulnerable = true;
        invulnerableTimer = INVULNERABLELENGTH;
    }

    private void FixedUpdate()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        //Chain drop
        if (GameManager.instance.playerDatas[playerIdx].chainTimer > 0)
        {
            GameManager.instance.playerDatas[playerIdx].chainTimer--;
            if (GameManager.instance.playerDatas[playerIdx].chainTimer == 0)
            {
                GameManager.instance.playerDatas[playerIdx].chainLevel = 0;
                ScoreManager.instance.UpdateChainMultiplier(playerIdx);
            }
        }

        //invulnerability flash
        if (isInvulnerable)
        {
            if (invulnerableTimer % 12 < 6)
            {
                spriteRenderer.material.SetColor("_Overbright",Color.black);
            }
            else
            {
                spriteRenderer.material.SetColor("_Overbright", Color.white);
            }
            invulnerableTimer--;
            if (invulnerableTimer <= 0)
            {
                isInvulnerable = false;
                spriteRenderer.material.SetColor("_Overbright", Color.black);
            }
        }

        //hit detection
        int maxColliders = 10;
        Collider2D[] hits = new Collider2D[maxColliders];

        //bullet hits
        Vector2 halfSize = new Vector2(0.03f, 0.04f);
        int noHits = Physics2D.OverlapBoxNonAlloc(transform.position, halfSize, 0, hits, layerMask);
        if (noHits > 0)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit && hit.gameObject.layer != pickupLayer)
                {
                    Hit();
                }
            }
        }
        

        //pickups and bullet grazing
        halfSize = new Vector2(0.15f, 0.21f);
        noHits = Physics2D.OverlapBoxNonAlloc(transform.position, halfSize, 0, hits, layerMask);
        if (noHits > 0)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit)
                {
                    if (hit.gameObject.layer == pickupLayer)
                    {
                        Pickup(hit.GetComponent<Pickup>());
                    }
                    else //bullet graze
                    if (craftData.beamCharge <= MAX_BEAM_CHARGE)
                    {
                        craftData.beamCharge++;
                        craftData.beamTimer++;
                    }
                }
            }
        }
        
        if (InputManager.instance && isAlive)
        {
            craftData.posX += InputManager.instance.playerState[playerIdx].mvmt.x * config.speed;
            craftData.posY += InputManager.instance.playerState[playerIdx].mvmt.y * config.speed;

            if (craftData.posX < -8.5f)
            {
                craftData.posX = -8.5f;
            }
            else if (craftData.posX > 8.5f)
            {
                craftData.posX = 8.5f;
            }

            if (craftData.posY < -5f)
            {
                craftData.posY = -5f;
            }
            else if (craftData.posY > 5f)
            {
                craftData.posY = 5f;
            }

            if (!GameManager.instance.progressWindow)
            {
                GameManager.instance.progressWindow = GameObject.FindObjectOfType<LevelProgress>();
            }

            newPos.x = craftData.posX;
            newPos.y = craftData.posY;
            if (GameManager.instance.progressWindow)
            {
                newPos.y += GameManager.instance.progressWindow.transform.position.y;
            }

            gameObject.transform.position = newPos;

            if (InputManager.instance.playerState[playerIdx].up && !InputManager.instance.playerState[playerIdx].down)
            {
                aftFlame1.SetActive(true);
                aftFlame2.SetActive(true);
                flame1.SetActive(false);
                flame2.SetActive(false);
            }
            else if (!InputManager.instance.playerState[playerIdx].up && InputManager.instance.playerState[playerIdx].down)
            {
                aftFlame1.SetActive(false);
                aftFlame2.SetActive(false);
                flame1.SetActive(false);
                flame2.SetActive(false);
            }
            else
            {
                aftFlame1.SetActive(false);
                aftFlame2.SetActive(false);
                flame1.SetActive(true);
                flame2.SetActive(true);
            }

            if (InputManager.instance.playerState[playerIdx].left)
            {
                anim.SetBool(leftBoolId, true);
            }
            else
            {
                anim.SetBool(leftBoolId, false);
            }

            if (InputManager.instance.playerState[playerIdx].right)
            {
                anim.SetBool(rightBoolId, true);
            }
            else
            {
                anim.SetBool(rightBoolId, false);
            }

            if (InputManager.instance.playerState[playerIdx].shoot)
            {
                ShotConfig shotConfig = config.shotLevel[craftData.shotPower];
                for (int i = 0; i < 5; i++)
                {
                    bulletSpawners[i].Shoot(shotConfig.spawnerSizes[i]);
                }

                for (int i = 0; i < craftData.noEnabledOptions; i++)
                {
                    if (options[i])
                    {
                        options[i].Shoot();
                    }
                }
            }
            if (InputManager.instance.StateDectivated(0, InputManager.Action.OPTIONS))
            {
                craftData.optionsLayout++;
                if (craftData.optionsLayout > 3)
                {
                    craftData.optionsLayout = 0;
                }
                setOptionsLayout(craftData.optionsLayout);
            }

            if (InputManager.instance.playerState[playerIdx].beam)
            {
                beam.Fire();
            }

            if (InputManager.instance.StateDectivated(0, InputManager.Action.BOMB))
            {
                FireBomb();
            }
        }
    }
    public void Hit()
    {
        if (!(isInvulnerable || (GameManager.instance && GameManager.instance.gameSession.hasInvincibility)) && isAlive)
        {
            Explode();
        }
    }

    public void Explode()
    {
        isAlive = false;
        GameManager.instance.playerDatas[playerIdx].lives--;
        StartCoroutine(Exploding());

        if (explodingNoise)
        {
            explodingNoise.Play();
        }
    }

    IEnumerator Exploding()
    {
        Color col = Color.white;
        for (float r = 0; r < 1; r += 0.3f)
        {
            col.g = 1 - r;
            col.b = 1 - r;
            spriteRenderer.color = col;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
        FXSys.instance.CraftExplosion(transform.position);
        GameManager.instance.playerCrafts[playerIdx] = null;

        bool allLivesDone = GameManager.instance.playerDatas[0].lives == 0; //Player1 lost all lives
        if (GameManager.instance.player2Exists) //If Player2 exists, game should be over iff it lost all lives
        {
            allLivesDone = allLivesDone && GameManager.instance.playerDatas[1].lives == 0;
        }
        if (allLivesDone)
        {
            GameOverMenu.instance.GameOver();
        }
        else
        {
            // Eject powerups and spawn next life
            CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
            int noOptionsToRespawn = Math.Max(craftData.noEnabledOptions - 1, 0);
            int noPowerupsToRespawn = craftData.shotPower - 1;
            int noBeamupsToRespawn = craftData.beamPower - 1;
            craftData.ResetData();

            for (int i = 0; i < noOptionsToRespawn; i++)
            {
                Pickup pickup = GameManager.instance.SpawnPickup(GameManager.instance.option, transform.position);
                pickup.transform.position += new Vector3(UnityEngine.Random.Range(-0.04f, 0.04f), UnityEngine.Random.Range(-0.04f, 0.04f), 0);
            }

            for (int i = 0; i < noPowerupsToRespawn; i++)
            {
                Pickup pickup = GameManager.instance.SpawnPickup(GameManager.instance.powerup, transform.position);
                pickup.transform.position += new Vector3(UnityEngine.Random.Range(-0.04f, 0.04f), UnityEngine.Random.Range(-0.04f, 0.04f), 0);
            }

            for (int i = 0; i < noBeamupsToRespawn; i++)
            {
                Pickup pickup = GameManager.instance.SpawnPickup(GameManager.instance.beamup, transform.position);
                pickup.transform.position += new Vector3(UnityEngine.Random.Range(-0.04f, 0.04f), UnityEngine.Random.Range(-0.04f, 0.04f), 0);
            }
            if (GameManager.instance.playerDatas[playerIdx].lives > 0)
            {
                GameManager.instance.DelayedRespawn(playerIdx);
            }
        }

        yield return null;
    }

    internal void AddOption(int surplusVal)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        if (craftData.noEnabledOptions < 4)
        {
            options[craftData.noEnabledOptions++].gameObject.SetActive(true);
        }
        else
        {
            ScoreManager.instance.PickupCollected(playerIdx, surplusVal);
        }
    }
    public void setOptionsLayout(int layoutIdx)
    {
        Debug.Assert(layoutIdx < 4 && layoutIdx >= 0);
        for(int i = 0; i < 4; i++)
        {
            switch (layoutIdx)
            {
                case 0:
                    options[i].gameObject.transform.position = optionMarkersL1[i].transform.position;
                    options[i].gameObject.transform.rotation = optionMarkersL1[i].transform.rotation;
                    break;
                case 1:
                    options[i].gameObject.transform.position = optionMarkersL2[i].transform.position;
                    options[i].gameObject.transform.rotation = optionMarkersL2[i].transform.rotation;
                    break;
                case 2:
                    options[i].gameObject.transform.position = optionMarkersL3[i].transform.position;
                    options[i].gameObject.transform.rotation = optionMarkersL3[i].transform.rotation;
                    break;
                case 3:
                    options[i].gameObject.transform.position = optionMarkersL4[i].transform.position;
                    options[i].gameObject.transform.rotation = optionMarkersL4[i].transform.rotation;
                    break;

            }
        }
    }
    public void IncreaseBeamStr(int surplusVal)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        if (craftData.beamPower < 3)
        {
            craftData.beamPower++;
            UpdateBeam();
        }
        else
        {
            ScoreManager.instance.PickupCollected(playerIdx, surplusVal);
        }
    }

    void UpdateBeam()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        beam.beamWidth = (craftData.beamPower + 2) * 0.3f;
    }

    void FireBomb()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        if (craftData.smallBombs > 0)
        {
            craftData.smallBombs--;
            Vector3 pos = transform.position;
            pos.y += 2f;
            if (bombSounds)
            {
                bombSounds.Play();
            }
            Bomb bomb = Instantiate(bombPrefab, pos, Quaternion.identity).GetComponent<Bomb>();
            if (bomb)
            {
                bomb.playerIdx = playerIdx;
            }
        }
    }

    private void Pickup(Pickup pickup)
    {
        if (pickup)
        {
            CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
            pickup.ProcessPickup(playerIdx, craftData);
        }
    }


    internal void Powerup(byte pow, int surplusVal)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        craftData.shotPower += pow;
        if (craftData.shotPower > 10)
        {
            ScoreManager.instance.PickupCollected(playerIdx, (craftData.shotPower - 10) * surplusVal);
            craftData.shotPower = 10;
        }
    }

    internal void IncreaseScore(int coinVal)
    {
        GameManager.instance.playerDatas[playerIdx].score += coinVal;
        GameManager.instance.playerDatas[playerIdx].stageScore += coinVal;
    }

    internal void OneUp(int surplusVal)
    {
        GameManager.instance.playerDatas[playerIdx].lives++;
        if (GameManager.instance.playerDatas[playerIdx].lives >= MAX_LIVES)
        {
            ScoreManager.instance.PickupCollected(playerIdx, surplusVal);
            GameManager.instance.playerDatas[playerIdx].lives = MAX_LIVES;
        }
    }

    internal void AddBomb(int bmbPow, int surplusVal)
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        switch (bmbPow)
        {
            case 1:
                {
                    craftData.smallBombs++;
                    if (craftData.smallBombs > MAX_SMALL_BOMBS)
                    {
                        ScoreManager.instance.PickupCollected(playerIdx, surplusVal);
                        craftData.smallBombs = MAX_SMALL_BOMBS;
                    }
                    break;
                }
            case 2:
                {
                    craftData.bigBombs++;
                    if (craftData.bigBombs > MAX_BIG_BOMBS)
                    {
                        ScoreManager.instance.PickupCollected(playerIdx, surplusVal);
                        craftData.bigBombs = MAX_BIG_BOMBS;
                    }
                    break;
                }
            default:
                {
                    Debug.LogError("Added bomb with unknown power: " + bmbPow);
                    break;
                }
        }
    }
    internal void AddMedal(int medalLvl, int medalVal)
    {
        ScoreManager.instance.MedalCollected(playerIdx,medalVal);
    }
}

public class CraftData
{
    public float posX;
    public float posY;

    public byte shotPower;

    public byte noEnabledOptions;
    public byte optionsLayout;

    public bool beamIsFiring;
    public byte beamPower;
    public byte beamCharge;
    public byte beamTimer;

    public byte smallBombs;
    public byte bigBombs;

    internal void Save(BinaryWriter bw)
    {
        bw.Write(shotPower);

        bw.Write(noEnabledOptions);
        bw.Write(optionsLayout);

        bw.Write(beamPower);
        bw.Write(beamCharge);

        bw.Write(smallBombs);
        bw.Write(bigBombs);
    }
    internal void Load(BinaryReader br)
    {
        shotPower = br.ReadByte();

        noEnabledOptions = br.ReadByte();
        optionsLayout = br.ReadByte();

        beamPower = br.ReadByte();
        beamCharge = br.ReadByte();

        smallBombs = br.ReadByte();
        bigBombs = br.ReadByte();

    }

    internal void ResetData()
    {
        posX = 0;
        posY = 0;

        shotPower = 0;

        noEnabledOptions = 0;
        optionsLayout = 0;

        beamCharge = 0;
        beamPower = 0;
        beamTimer = 0;
        beamIsFiring = false;

        smallBombs = 3;
        bigBombs = 0;
    }
}