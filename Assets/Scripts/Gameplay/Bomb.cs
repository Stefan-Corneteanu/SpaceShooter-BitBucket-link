using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float radius = 0.1f;
    public int power = 50;
    public byte playerIdx = 2; //0 -> player1, 1 -> player2, 2 or anything else: enemy
}
