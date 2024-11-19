using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public EnemyPattern[] patterns = null;
    public int noEnemies = 0;
    public float[] delays = null;

    public int waveBonus = 0;
    public bool spawnCyclicPickups = false;
    public Pickup[] spawnSpecificPickups;
    private void Start()
    {
        if (patterns != null)
        {
            noEnemies = Array.FindAll(patterns, pattern => pattern.ShouldSpawn()).Length;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(SpawnWave());
    }

    public IEnumerator SpawnWave()
    {
        Session.Difficulty diff = GameManager.instance.gameSession.diff;
        int i = 0;
        foreach (EnemyPattern pattern in patterns)
        {
            if (pattern)
            {
                pattern.owningWave = this;
                if (delays != null && i < delays.Length)
                {
                    yield return new WaitForSeconds(delays[i]);
                }
                if (pattern.ShouldSpawn())
                {
                    pattern.Spawn();
                }
            }
            i++;
        }
        yield return null;
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, collider.size);
        }
    }

    internal void EnemyDestroyed(Vector3 pos, int playerIdx)
    {
        noEnemies--;
        if (noEnemies == 0)
        {
            ScoreManager.instance.ShootableDestroyed(playerIdx, waveBonus);
            if (spawnCyclicPickups)
            {
                Pickup spawn = GameManager.instance.GetNextDrop();
                GameManager.instance.SpawnPickup(spawn, pos);
            }

            foreach(Pickup pickup in spawnSpecificPickups)
            {
                GameManager.instance.SpawnPickup(pickup, pos);
            }
        }
    }
}
