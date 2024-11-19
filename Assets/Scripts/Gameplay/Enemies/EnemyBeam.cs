using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float beamWidth = 1;
    private int layerMask = 0;
    public GameObject beamFlash = null;
    private bool isFiring = false;

    public GameObject endPoint = null;

    private bool charging = false;

    private const int FULL_CHARGE_TIME = 60;
    private int chargeTimer = FULL_CHARGE_TIME;

    public AudioSource audioSource = null;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") 
                    & ~LayerMask.GetMask("GroundEnemy")
                    & ~LayerMask.GetMask("EnemyBullets");
    }

    public void Fire()
    {
        if (!isFiring)
        {
            isFiring = true;
            charging = true;
            if (audioSource)
            {
                audioSource.Play();
            }

            UpdateBeam();
            float scale = beamWidth;
            gameObject.SetActive(true);
            if (beamFlash)
            {
                beamFlash.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

    }

    public void StopFiring()
    {
        isFiring = false;
        charging = false;
        if (audioSource)
        {
            audioSource.Stop();
        }
        gameObject.SetActive(false);
        if (beamFlash)
        {
            beamFlash.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (isFiring)
        {
            UpdateBeam();
        }
    }

    void UpdateBeam()
    {
        if (!charging)
        {
            int maxColliders = 20;
            Collider2D[] hits = new Collider2D[maxColliders];

            Vector2 center = Vector2.Lerp(transform.position, endPoint.transform.position, 0.5f);
            Vector2 halfSize = new Vector2(beamWidth * 0.5f, (endPoint.transform.position - transform.position).magnitude * 0.5f);

            int noHits = Physics2D.OverlapBoxNonAlloc(center, halfSize, transform.eulerAngles.z, hits, layerMask);

            for (int i = 0; i < noHits; i++)
            {
                Craft craft = hits[i].GetComponent<Craft>();
                if (craft)
                {
                    craft.Hit();
                }
            }
            lineRenderer.startWidth = beamWidth;
            lineRenderer.endWidth = beamWidth;
        }
        else
        {
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            chargeTimer--;
            if (chargeTimer <= 0)
            {
                charging = false;
                chargeTimer = FULL_CHARGE_TIME;

                if (beamFlash)
                {
                    beamFlash.SetActive(true);
                }
            }
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint.transform.position);
    }
}
