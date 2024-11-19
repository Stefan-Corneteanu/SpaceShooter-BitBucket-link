using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSys : MonoBehaviour
{
    public static FXSys instance = null;

    public GameObject craftExplosionPrefab = null;
    public ParticleSystem craftExplosionParticleSystemPrefab = null;
    public ParticleSystem craftDebrisExplosionParticleSystemPrefab = null;
    public ParticleSystem hitParticleSystemPrefab = null;

    public GameObject largeExplosionPrefab = null;
    public GameObject smallExplosionPrefab = null;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Error: Trying to create an extra Effects System");
            Destroy(gameObject);
        }
    }

    public void CraftExplosion(Vector3 pos)
    {
        Instantiate(craftExplosionPrefab,pos,Quaternion.identity);
        Instantiate(craftExplosionParticleSystemPrefab, pos, Quaternion.identity);
        Instantiate(craftDebrisExplosionParticleSystemPrefab, pos, Quaternion.identity);
    }

    public void SpawnSparks(Vector3 pos)
    {
        Quaternion angle = Quaternion.Euler(0, 0, 45f);
        Instantiate(hitParticleSystemPrefab, pos, angle);
    }

    public void SpawnLargeExplosion(Vector3 pos)
    {
        Instantiate(largeExplosionPrefab, pos, Quaternion.identity);
    }

    public void SpawnSmallExplosion(Vector3 pos)
    {
        Instantiate(smallExplosionPrefab, pos, Quaternion.identity);
    }
}
