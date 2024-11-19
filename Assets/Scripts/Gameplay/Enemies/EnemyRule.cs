using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyRule
{
    public bool isTriggered = false;
    public int noPartsRequired;
    public List<EnemyPart> partsToCheck = new List<EnemyPart>();
    [Space(10)]
    [Header("--On Rule Triggered--")]
    [Space(10)]
    public UnityEvent ruleEvents = null;
    public List<int> eventDelays = new List<int>();
}
