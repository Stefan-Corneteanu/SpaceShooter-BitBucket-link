using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float beamWidth = 1f;
    public Craft craft = null;
    private int layerMask = 0;
    public GameObject beamFlash = null;
    public byte playerIdx = 2; //0 -> player1, 1 -> player2, 2 or anything else: enemy

    public GameObject[] beamHits = new GameObject[5];
    const int MIN_CHARGE = 10;

    public AudioSource audioSource = null;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("PlayerBullets");
    }

    public void Fire()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        if (!craftData.beamIsFiring)
        {
            if (craftData.beamCharge >= MIN_CHARGE)
            {
                craftData.beamIsFiring = true;
                craftData.beamTimer = craftData.beamCharge;
                craftData.beamCharge = 0;
                UpdateBeam();
                float scale = beamWidth;
                beamFlash.transform.localScale = new Vector3(scale, scale, 1);
                gameObject.SetActive(true);
                beamFlash.SetActive(true);
                if (audioSource)
                {
                    audioSource.Play();
                }
            }
            else
            {
                UpdateBeam();
            }
        }

    }

    private void FixedUpdate()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        if (craftData.beamIsFiring)
        {
            UpdateBeam();
        }
    }

    void HideHits()
    {
        for (int i = 0; i < 5; i++)
        {
            beamHits[i].SetActive(false);
        }
    }

    void UpdateBeam()
    {
        CraftData craftData = GameManager.instance.gameSession.craftDatas[playerIdx];
        if (craftData.beamTimer > 0)
        {
            craftData.beamTimer--;
        }

        if (craftData.beamTimer <= 0)
        {
            craftData.beamIsFiring = false;
            audioSource.Stop();
            gameObject.SetActive(false);
            beamFlash.SetActive(false);
            HideHits();
            return;
        }

        float topY = 5;
        if (GameManager.instance && GameManager.instance.progressWindow)
        {
            topY += GameManager.instance.progressWindow.transform.position.y;
        }

        int maxColliders = 20;
        Collider2D[] hits = new Collider2D[maxColliders];
        Vector2 halfSize = new Vector2(beamWidth*0.5f, (topY-craft.transform.position.y)*0.5f);
        float middleY = (craft.transform.position.y + topY) * 0.5f;
        Vector3 center = new Vector3(craft.transform.position.x, middleY, 0);

        int noHits = Physics2D.OverlapBoxNonAlloc(center,halfSize, 0, hits, layerMask);

        float lowest = topY;

        Shootable lowestShootable = null;
        Collider2D lowestCollider = null;
        const int MAX_RAY_HITS = 10;

        if (noHits > 0)
        {
            //Find lowest hit
            for (int i = 0; i < noHits; i++)
            {
                Shootable shootable = hits[i].GetComponent<Shootable>();

                if (shootable && shootable.damagedByBeams)
                {
                    RaycastHit2D[] hitInfo = new RaycastHit2D[MAX_RAY_HITS];
                    Vector2 ray =  Vector2.up;
                    float height = (topY - craft.transform.position.y);
                    if (hits[i].Raycast(ray, hitInfo, height) > 0)
                    {
                        if (hitInfo[0].point.y < lowest)
                        {
                            lowest = hitInfo[0].point.y;
                            lowestShootable = shootable;
                            lowestCollider = hits[i];
                        }
                    }
                }
            }

            //Find hits on collider
            if (lowestShootable != null)
            {
                //fire 5 rays for each hit
                Vector3 start = craft.transform.position;
                start.x -= 2*(beamWidth / 5);
                for (int i = 0; i < 5; i++)
                {
                    RaycastHit2D[] hitInfo = new RaycastHit2D[MAX_RAY_HITS];
                    Vector2 ray = Vector2.up;

                    if (lowestCollider.Raycast(ray, hitInfo, 10) > 0)
                    {
                        Vector3 pos = hitInfo[0].point;
                        pos.x += Random.Range(-0.01f, 0.01f);
                        pos.y += Random.Range(-0.01f, 0.01f);
                        beamHits[i].transform.position = pos;
                        lowestShootable.TakeDamage(craftData.beamPower+1, playerIdx);
                        beamHits[i].SetActive(true);
                    }
                    else
                    {
                        beamHits[i].SetActive(false);
                    }
                    start.x += (beamWidth / 5);
                }
            }
            else
            {
                HideHits();
            }
        }
        else
        {
            HideHits();
        }

        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.SetPosition(0, transform.position);
        Vector3 top = transform.position;
        top.y = lowest;
        lineRenderer.SetPosition(1, top);
    }
}
